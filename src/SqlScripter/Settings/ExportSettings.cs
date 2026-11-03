using System.ComponentModel;
using Spectre.Cli;

namespace SqlScripter.Settings;

public class ExportSettings : CommandSettings
{
    public ExportSettings()
    {
        OutputDirectory = "./";
    }

    [CommandOption("-d|--database <DatabaseName>")]
    [Description("Specifies the database name.")]
    public string DatabaseName { get; set; }

    [CommandOption("-s|--server <Server>")]
    [Description("Specifies the server name.")]
    public string ServerName { get; set; }

    [CommandOption("-l|--login <Login>")]
    [Description("Database login.")]
    public string Login { get; set; }

    [CommandOption("-p|--password <Password>")]
    [Description("Database password.")]
    public string Password { get; set; }

    [CommandOption("--trust-server-certificate")]
    [Description("Trust the server's TLS certificate without validating it (e.g. self-signed certs).")]
    public bool TrustServerCertificate { get; set; }

    [CommandOption("--no-dependencies")]
    [Description("Skip dependency discovery (much faster on large databases). Scripts tables, then foreign keys, then views.")]
    public bool NoDependencies { get; set; }


    [CommandOption("-o|--output <OutputDirectory>")]
    [Description("Output directory.")]
    public string OutputDirectory { get; set; }
}