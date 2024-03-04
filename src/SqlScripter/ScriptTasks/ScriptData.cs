using System;
using System.IO;
using Microsoft.SqlServer.Management.Smo;

namespace SqlScripter.ScriptTasks;

public class ScriptData : SqlScriptTask
{
    public string TableName { get; set; }

    public override void Run()
    {
        var scripter = new Scripter(DatabaseConnection.Server)
        {
            Options =
            {
                ScriptData = true,
                ScriptSchema = false
            }
        };


        var table = DatabaseConnection.Database.Tables[TableName];

        var sc = scripter.EnumScript(new SqlSmoObject[] { table });


        var filePath = Path.Join(OutputDirectory, $"{TableName}.sql");
        if (File.Exists(filePath)) File.Delete(filePath);

        using var file = File.OpenWrite(filePath);
        using var sw = new StreamWriter(file);
        
        foreach (var item in sc) sw.WriteLine($"{item} {Environment.NewLine}{Environment.NewLine}");

        sw.Close();
        file.Close();
    }
}