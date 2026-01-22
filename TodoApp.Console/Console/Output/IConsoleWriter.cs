namespace TodoApp.Console.Console.Output;

public interface IConsoleWriter
{
    void WriteSuccess(string message);
    void WriteError(string message);
    void WriteInfo(string message);
    void WriteWarning(string message);
    void WriteLine(string message);
    void WriteLine();
    void Write(string message);
    void WriteHeader();
    void WriteBox(string content);
    void WritePreview(string title, Dictionary<string, string> data);
    void WriteProgressBar(decimal percent);
}
