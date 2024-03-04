namespace SqlScripter;

public abstract class SqlScriptTask
{
    public string OutputDirectory { get; set; }
    public DatabaseConnection DatabaseConnection { get; set; }

    public abstract void Run();
}