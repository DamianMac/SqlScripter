using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.SqlServer.Management.Sdk.Sfc;
using Microsoft.SqlServer.Management.Smo;

namespace SqlScripter.ScriptTasks
{
    public class ScriptAllTables : SqlScriptTask
    {

        public override void Run()
        {
            var scripter = new Microsoft.SqlServer.Management.Smo.Scripter(DatabaseConnection.Server);
            scripter.Options.DriAllConstraints = true;
            scripter.Options.WithDependencies = true;
            scripter.Options.Indexes = true;
            scripter.Options.IncludeIfNotExists = true;
            
            var urns = GetTablesUrns(DatabaseConnection.Database).ToArray();
            
            var sc = scripter.Script(urns);


            string filePath = Path.Join(OutputDirectory, "Schema.sql");
            if ( File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            using(var file = File.OpenWrite(filePath))
            using(var sw = new StreamWriter(file))
            {

                foreach (var item in sc)
                {
                    sw.WriteLine($"{item} {Environment.NewLine}GO{Environment.NewLine}");
                }

                sw.Close();
                file.Close();

            }

        }

        private IEnumerable<Urn> GetTablesUrns(Microsoft.SqlServer.Management.Smo.Database database)
        {
            foreach (Table table in database.Tables)
            {
                if (!table.IsSystemObject)
                {
                    yield return table.Urn;
                }
            }
        }

    }

}