using Spectre.Cli;
using SqlScripter.ScriptTasks;
using SqlScripter.Settings;

namespace SqlScripter.Commands;

public class ExportSchemaCommand : Command<ExportSettings>
{
    public override int Execute(CommandContext context, ExportSettings settings)
    {
        using var conn = new DatabaseConnection(settings.ServerName, settings.DatabaseName, settings.Login,
            settings.Password);
        var task = new ScriptAllTables
        {
            DatabaseConnection = conn,
            OutputDirectory = settings.OutputDirectory
        };
        task.Run();
        return 0;
    }
}