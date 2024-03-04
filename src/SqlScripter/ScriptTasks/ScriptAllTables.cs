using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.SqlServer.Management.Sdk.Sfc;
using Microsoft.SqlServer.Management.Smo;

namespace SqlScripter.ScriptTasks;

public class ScriptAllTables : SqlScriptTask
{
    public override void Run()
    {
        var scripter = new Scripter(DatabaseConnection.Server)
        {
            Options =
            {
                DriAllConstraints = true,
                WithDependencies = true,
                Indexes = true,
                IncludeIfNotExists = true
            }
        };

        var urns = GetTablesUrns(DatabaseConnection.Database)
            .Union(GetViewsUrns(DatabaseConnection.Database))
            .ToArray();

        var sc = scripter.Script(urns);


        var filePath = Path.Join(OutputDirectory, "Schema.sql");
        if (File.Exists(filePath)) File.Delete(filePath);

        using var file = File.OpenWrite(filePath);
        using var sw = new StreamWriter(file);
        foreach (var item in sc) sw.WriteLine($"{item} {Environment.NewLine}GO{Environment.NewLine}");

        sw.Close();
        file.Close();
    }

    private IEnumerable<Urn> GetTablesUrns(Database database)
    {
        foreach (Table table in database.Tables)
            if (!table.IsSystemObject)
                yield return table.Urn;
    }

    private IEnumerable<Urn> GetViewsUrns(Database database)
    {
        foreach (View view in database.Views)
            if (!view.IsSystemObject)
                yield return view.Urn;
    }
}