# SQL Scripter

A .NET command line utility that scripts DDL and data from a SQL Server or Azure SQL database, using SQL Server Management Objects (SMO).

Requires the .NET 9 SDK.

## Usage

Run from `src/SqlScripter`:

```
dotnet run -- <command> [options]
```

Or build it and run the binary directly:

```
dotnet build -c Release
./bin/Release/net90/SqlScripter <command> [options]
```

## Commands

| Command | Output |
| --- | --- |
| `export schema` | `Schema.sql`: all user tables (with constraints and indexes) and views |
| `export storedprocs` | `StoredProcedures/<name>.sql`: one file per stored procedure, each with a drop and a create |
| `export udfs` | `UserDefinedFunctions/<name>.sql`: one file per user-defined function |
| `generate inserts` | `<table>.sql`: INSERT statements for every row in a single table |

Existing output files are overwritten.

## Options

These options apply to every command:

| Option | Description |
| --- | --- |
| `-s`, `--server <Server>` | Server name, e.g. `localhost` or `myserver.database.windows.net` |
| `-d`, `--database <DatabaseName>` | Database name |
| `-l`, `--login <Login>` | SQL login |
| `-p`, `--password <Password>` | Password |
| `-o`, `--output <OutputDirectory>` | Output directory. Defaults to the current directory |
| `--trust-server-certificate` | Skip validation of the server's TLS certificate. Use for local servers with self-signed certificates |

Only SQL authentication is supported.

### `export schema`

| Option | Description |
| --- | --- |
| `--no-dependencies` | Skip SMO's dependency discovery. See below |

By default SMO works out the dependency order between all tables and views before scripting them. On large databases (thousands of tables) this can take a very long time, especially on Azure SQL.

With `--no-dependencies`, the tool skips that step and runs three passes instead:

1. Tables, with indexes and constraints but no foreign keys
2. Foreign keys, as `ALTER TABLE` statements
3. Views

The output still runs in order, with one exception: views that reference other views aren't sorted, so a view may appear before one it depends on.

### `generate inserts`

| Option | Description |
| --- | --- |
| `-t`, `--table <TableName>` | The table to script data from. Must be in the default schema |

## Examples

```
dotnet run -- export schema -s localhost -d mydb -l sa -p 'secret' --trust-server-certificate

dotnet run -- export schema -s myserver.database.windows.net -d mydb -l admin -p 'secret' -o ./out --no-dependencies

dotnet run -- export storedprocs -s localhost -d mydb -l sa -p 'secret' -o ./out

dotnet run -- generate inserts -s localhost -d mydb -l sa -p 'secret' -t Customers
```

Put the password in single quotes. In zsh and bash, an unquoted `!` in a password triggers history expansion and the wrong password gets sent.

Use `--help` on any command to list its options, e.g. `dotnet run -- export schema --help`.

## Progress and errors

`export schema` writes timestamped progress to stderr, so you can see a long run is still moving. If a connection fails, the full chain of inner exceptions is printed, which usually shows the real cause (login failure, TLS, firewall).
