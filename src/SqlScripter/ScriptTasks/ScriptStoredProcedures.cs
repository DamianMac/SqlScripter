using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.SqlServer.Management.Sdk.Sfc;
using Microsoft.SqlServer.Management.Smo;

namespace SqlScripter.ScriptTasks
{
    public class ScriptStoredProcedures : SqlScriptTask
    {

        public override void Run()
        {

            var path = Path.Join(OutputDirectory, "StoredProcedures");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            var scripter = new Microsoft.SqlServer.Management.Smo.Scripter(DatabaseConnection.Server);
            scripter.Options.ScriptDrops = true;
            scripter.Options.IncludeIfNotExists = true;

            foreach (var proc in GetStoredProcedures(DatabaseConnection.Database))
            {
                string filePath = Path.Join(path, $"{proc.Name}.sql");
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }

                try
                {
                    var script = proc.Script();
                    var drops = proc.Script(new ScriptingOptions
                    {
                        ScriptDrops = true,
                        IncludeIfNotExists = true

                    });

                    using (var file = File.OpenWrite(filePath))
                    using (var sw = new StreamWriter(file))
                    {

                        foreach (var item in drops)
                        {
                            sw.WriteLine($"{item}{Environment.NewLine}GO{Environment.NewLine}");
                        }

                        foreach (var item in script)
                        {
                            sw.WriteLine($"{item} {Environment.NewLine}GO{Environment.NewLine}");
                        }

                        sw.Close();
                        file.Close();

                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }

            }

        }

        private IEnumerable<StoredProcedure> GetStoredProcedures(Microsoft.SqlServer.Management.Smo.Database database)
        {
            foreach (StoredProcedure proc in database.StoredProcedures)
            {
                if (!proc.IsSystemObject)
                {
                    yield return proc;
                }
            }
        }
    }
}