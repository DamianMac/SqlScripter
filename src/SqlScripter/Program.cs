using Spectre.Cli;
using SqlScripter.Commands;
using SqlScripter.Settings;

namespace SqlScripter;

internal class Program
{
    public static int Main(string[] args)
    {
        var app = new CommandApp();
        app.Configure(c =>
        {
            c.ValidateExamples();
            c.PropagateExceptions();

            c.AddBranch<ExportSettings>("export", export =>
            {
                export.SetDescription("Export tool for things");

                export.AddCommand<ExportSchemaCommand>("schema");
                export.AddCommand<ExportStoredProcsCommand>("storedprocs");
                export.AddCommand<ExportUDFCommand>("udfs");
            });

            c.AddBranch<GenerateInsertsSettings>("generate", g =>
            {
                g.SetDescription("Code generation tools");
                g.AddCommand<GenerateInsertsCommand>("inserts");
            });
        });
        try
        {
            return app.Run(args);
        }
        catch (System.Exception ex)
        {
            for (var e = ex; e != null; e = e.InnerException)
                System.Console.Error.WriteLine($"{e.GetType().Name}: {e.Message}");
            return -1;
        }
    }
}