using System;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;

namespace SqlScripter
{
    public class DatabaseConnection : IDisposable
    {
        public Database Database { get; set; }
        public Server Server { get; set; }

        private ServerConnection connection;
        
        public DatabaseConnection(string serverName, string databaseName)
        {
            connection = new ServerConnection(serverName);
            Init(connection, databaseName);
        }

        public DatabaseConnection(string serverName, string databaseName, string login, string password)
        {
            connection = new ServerConnection(serverName, login, password);
            Init(connection, databaseName);

        }

        private void Init(ServerConnection connection, string databaseName)
        {
            Server = new Microsoft.SqlServer.Management.Smo.Server(connection);
            Database = Server.Databases[databaseName];
        }

        public void Dispose()
        {
            connection.Disconnect();
        }
    }
}