using System;
using System.Linq;

namespace SqlScripter
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            var tasks = new []{typeof(ScriptTasks.ScriptAllTables), typeof(ScriptTasks.ScriptStoredProcedures), typeof(ScriptTasks.ScriptUserDefinedFunctions)};

            using(var conn = new DatabaseConnection("localhost", "db", "user", "password!"))
            {
                var task = (SqlScriptTask)Activator.CreateInstance(tasks.Last());
                task.DatabaseConnection = conn;
                task.OutputDirectory = "./";
                task.Run();

            }

            Console.WriteLine("Done");
        }
    }
}
