namespace SqlScripter
{
    public abstract class SqlScriptTask
    {

        public abstract void Run();

        public string OutputDirectory { get; set; }
        public DatabaseConnection DatabaseConnection { get; set; }
    }
}