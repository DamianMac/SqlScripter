using System;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;

namespace SqlScripter;

public class DatabaseConnection : IDisposable
{
    private readonly ServerConnection connection;

    public DatabaseConnection(string serverName, string databaseName, bool trustServerCertificate = false)
    {
        connection = new ServerConnection(serverName)
        {
            TrustServerCertificate = trustServerCertificate
        };
        Init(connection, databaseName);
    }

    public DatabaseConnection(string serverName, string databaseName, string login, string password,
        bool trustServerCertificate = false)
    {
        connection = new ServerConnection(serverName, login, password)
        {
            TrustServerCertificate = trustServerCertificate
        };
        Init(connection, databaseName);
    }

    public Database Database { get; set; }
    public Server Server { get; set; }

    public void Dispose()
    {
        connection.Disconnect();
    }

    private void Init(ServerConnection connection, string databaseName)
    {
        Server = new Server(connection);
        Database = Server.Databases[databaseName];
    }
}
