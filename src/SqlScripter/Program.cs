using System;
using System.Linq;
using Spectre.Cli;
using SqlScripter.Commands;
using SqlScripter.Settings;

namespace SqlScripter
{
    class Program
    {
        public static int Main(string[] args)
        {
            var app = new CommandApp();
            app.Configure(c => 
            {
                c.ValidateExamples();
                c.AddBranch<ExportSettings>("export", export => {
                    export.SetDescription("ssss");

                     export.AddCommand<ExportSchemaCommand>("schema");

                });

            });
            return app.Run(args);

            // Console.WriteLine("Hello World!");

            // var tasks = new []{typeof(ScriptTasks.ScriptAllTables), typeof(ScriptTasks.ScriptStoredProcedures), typeof(ScriptTasks.ScriptUserDefinedFunctions)};

            // using(var conn = new DatabaseConnection("localhost", "db", "user", "password!"))
            // {
            //     var task = (SqlScriptTask)Activator.CreateInstance(tasks.Last());
            //     task.DatabaseConnection = conn;
            //     task.OutputDirectory = "./";
            //     task.Run();

            // }

            // Console.WriteLine("Done");
        }
    }
}
