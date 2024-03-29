using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.SqlServer.Management.Smo;

namespace SqlScripter.ScriptTasks;

public class ScriptUserDefinedFunctions : SqlScriptTask
{
    public override void Run()
    {
        var path = Path.Join(OutputDirectory, "UserDefinedFunctions");
        if (!Directory.Exists(path)) Directory.CreateDirectory(path);

        var scripter = new Scripter(DatabaseConnection.Server)
        {
            Options =
            {
                ScriptDrops = true,
                IncludeIfNotExists = true
            }
        };

        foreach (var func in GetFunctions(DatabaseConnection.Database))
        {
            var filePath = Path.Join(path, $"{func.Name}.sql");
            if (File.Exists(filePath)) File.Delete(filePath);

            try
            {
                var script = func.Script();
                var drops = func.Script(new ScriptingOptions
                {
                    ScriptDrops = true,
                    IncludeIfNotExists = true
                });

                using var file = File.OpenWrite(filePath);
                using var sw = new StreamWriter(file);
                foreach (var item in drops) sw.WriteLine($"{item}{Environment.NewLine}GO{Environment.NewLine}");

                foreach (var item in script) sw.WriteLine($"{item} {Environment.NewLine}GO{Environment.NewLine}");

                sw.Close();
                file.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }

    private IEnumerable<UserDefinedFunction> GetFunctions(Database database)
    {
        foreach (UserDefinedFunction function in database.UserDefinedFunctions)
            if (!function.IsSystemObject)
                yield return function;
    }
}