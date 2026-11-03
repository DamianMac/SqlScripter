using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.SqlServer.Management.Sdk.Sfc;
using Microsoft.SqlServer.Management.Smo;

namespace SqlScripter.ScriptTasks;

public class ScriptAllTables : SqlScriptTask
{
    /// <summary>
    /// Skip SMO dependency discovery. Tables are scripted without foreign keys,
    /// then all foreign keys, then views, so the output still runs in order.
    /// </summary>
    public bool NoDependencies { get; set; }

    public override void Run()
    {
        var server = DatabaseConnection.Server;
        var database = DatabaseConnection.Database;

        // Without this SMO issues a separate query per object to read IsSystemObject.
        server.SetDefaultInitFields(typeof(Table), nameof(Table.IsSystemObject));
        server.SetDefaultInitFields(typeof(View), nameof(View.IsSystemObject));

        var statements = NoDependencies ? ScriptWithoutDependencies(server, database) : ScriptWithDependencies(server, database);

        var filePath = Path.Join(OutputDirectory, "Schema.sql");
        if (File.Exists(filePath)) File.Delete(filePath);

        using var file = File.OpenWrite(filePath);
        using var sw = new StreamWriter(file);
        foreach (var item in statements) sw.WriteLine($"{item} {Environment.NewLine}GO{Environment.NewLine}");

        sw.Close();
        file.Close();
        Log($"Wrote {filePath}");
    }

    private List<string> ScriptWithDependencies(Server server, Database database)
    {
        var scripter = CreateScripter(server);
        scripter.Options.DriAllConstraints = true;
        scripter.Options.WithDependencies = true;
        scripter.Options.Indexes = true;

        var discovered = 0;
        scripter.DiscoveryProgress += (_, e) =>
        {
            if (++discovered % 100 == 0) Log($"Discovering dependencies: {discovered} objects visited (at {e.Current})");
        };

        var tableUrns = EnumerateTables(database).Select(t => t.Urn).ToArray();
        var viewUrns = EnumerateViews(database).Select(v => v.Urn).ToArray();
        var urns = tableUrns.Concat(viewUrns).ToArray();

        Log($"Scripting {urns.Length} objects with dependencies (discovery can take a while)...");
        var sc = scripter.Script(urns);
        Log($"Scripting complete: {sc.Count} statements.");
        return sc.Cast<string>().ToList();
    }

    private List<string> ScriptWithoutDependencies(Server server, Database database)
    {
        var results = new List<string>();

        // Pass 1: tables with indexes and constraints, but no foreign keys.
        var tableScripter = CreateScripter(server);
        tableScripter.Options.DriAllConstraints = true;
        tableScripter.Options.DriForeignKeys = false;
        tableScripter.Options.Indexes = true;

        // Load tables plus their columns/indexes/keys in bulk rather than per table.
        Log("Prefetching table metadata...");
        database.PrefetchObjects(typeof(Table), tableScripter.Options);

        var tables = EnumerateTables(database).ToList();
        Log($"Pass 1/3: scripting {tables.Count} tables (no foreign keys)...");
        results.AddRange(Script(tableScripter, tables.Select(t => t.Urn)));

        // Pass 2: foreign keys, once every table exists.
        var fkUrns = tables.SelectMany(t => t.ForeignKeys.Cast<ForeignKey>()).Select(fk => fk.Urn).ToList();
        Log($"Pass 2/3: scripting {fkUrns.Count} foreign keys...");
        results.AddRange(Script(CreateScripter(server), fkUrns));

        // Pass 3: views. Views that reference other views may still need reordering.
        var views = EnumerateViews(database).ToList();
        Log($"Pass 3/3: scripting {views.Count} views...");
        results.AddRange(Script(CreateScripter(server), views.Select(v => v.Urn)));

        Log($"Scripting complete: {results.Count} statements.");
        return results;
    }

    private IEnumerable<string> Script(Scripter scripter, IEnumerable<Urn> urns)
    {
        var array = urns.ToArray();
        return array.Length == 0 ? Enumerable.Empty<string>() : scripter.Script(array).Cast<string>();
    }

    private Scripter CreateScripter(Server server)
    {
        var scripter = new Scripter(server) { Options = { IncludeIfNotExists = true } };
        scripter.ScriptingProgress += (_, e) =>
        {
            if (e.Total % 50 == 0 || e.Total == e.TotalCount) Log($"  Scripting {e.Total}/{e.TotalCount}: {e.Current}");
        };
        scripter.ScriptingError += (_, e) => Log($"Scripting error at {e.Current}: {e.InnerException?.Message}");
        return scripter;
    }

    private IEnumerable<Table> EnumerateTables(Database database)
    {
        Log("Enumerating tables...");
        var tables = database.Tables.Cast<Table>().Where(t => !t.IsSystemObject).ToList();
        Log($"Found {tables.Count} tables.");
        return tables;
    }

    private IEnumerable<View> EnumerateViews(Database database)
    {
        Log("Enumerating views...");
        var views = database.Views.Cast<View>().Where(v => !v.IsSystemObject).ToList();
        Log($"Found {views.Count} views.");
        return views;
    }
}
