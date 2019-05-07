using System.ComponentModel;
using Spectre.Cli;

namespace SqlScripter.Settings
{
    public class ExportSettings : CommandSettings
    {
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
    }
}