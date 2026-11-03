using System;
using System.Diagnostics;

namespace SqlScripter;

public abstract class SqlScriptTask
{
    private readonly Stopwatch stopwatch = Stopwatch.StartNew();

    public string OutputDirectory { get; set; }
    public DatabaseConnection DatabaseConnection { get; set; }

    public abstract void Run();

    // Progress goes to stderr so it never mixes with scripted output.
    protected void Log(string message) =>
        Console.Error.WriteLine($"[{stopwatch.Elapsed:hh\\:mm\\:ss}] {message}");
}
