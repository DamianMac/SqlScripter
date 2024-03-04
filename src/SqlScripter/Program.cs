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
        return app.Run(args);
    }
}