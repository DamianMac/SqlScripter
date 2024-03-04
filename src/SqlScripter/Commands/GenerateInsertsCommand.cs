using Spectre.Cli;
using SqlScripter.ScriptTasks;
using SqlScripter.Settings;

namespace SqlScripter.Commands;

public class GenerateInsertsCommand : Command<GenerateInsertsSettings>
{
    public override int Execute(CommandContext context, GenerateInsertsSettings settings)
    {
        using var conn = new DatabaseConnection(settings.ServerName, settings.DatabaseName, settings.Login,
            settings.Password);

        var task = new ScriptData
        {
            TableName = settings.TableName,
            DatabaseConnection = conn,
            OutputDirectory = settings.OutputDirectory
        };
        task.Run();
        return 0;
    }
}