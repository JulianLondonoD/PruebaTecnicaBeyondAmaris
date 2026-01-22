namespace TodoApp.Console.Console.Output;

public class ConsoleWriter : IConsoleWriter
{
    public void WriteSuccess(string message)
    {
        System.Console.ForegroundColor = ConsoleColor.Green;
        System.Console.WriteLine($"✓ {message}");
        System.Console.ResetColor();
    }

    public void WriteError(string message)
    {
        System.Console.ForegroundColor = ConsoleColor.Red;
        System.Console.WriteLine($"✗ {message}");
        System.Console.ResetColor();
    }

    public void WriteInfo(string message)
    {
        System.Console.ForegroundColor = ConsoleColor.Cyan;
        System.Console.WriteLine($"ℹ {message}");
        System.Console.ResetColor();
    }

    public void WriteWarning(string message)
    {
        System.Console.ForegroundColor = ConsoleColor.Yellow;
        System.Console.WriteLine($"⚠ {message}");
        System.Console.ResetColor();
    }

    public void WriteLine(string message)
    {
        System.Console.WriteLine(message);
    }

    public void WriteLine()
    {
        System.Console.WriteLine();
    }

    public void Write(string message)
    {
        System.Console.Write(message);
    }

    public void WriteHeader()
    {
        System.Console.ForegroundColor = ConsoleColor.Cyan;
        System.Console.WriteLine("╔══════════════════════════════════════════╗");
        System.Console.WriteLine("║         📝 Aplicación de Tareas          ║");
        System.Console.WriteLine("╚══════════════════════════════════════════╝");
        System.Console.ResetColor();
        System.Console.WriteLine();
    }

    public void WriteBox(string content)
    {
        System.Console.WriteLine("┌─────────────────────────────────────┐");
        System.Console.WriteLine($"│ {content.PadRight(36)}│");
        System.Console.WriteLine("└─────────────────────────────────────┘");
    }

    public void WritePreview(string title, Dictionary<string, string> data)
    {
        System.Console.ForegroundColor = ConsoleColor.Cyan;
        System.Console.WriteLine($"\nℹ {title}");
        System.Console.ResetColor();
        
        foreach (var item in data)
        {
            System.Console.WriteLine($"  {item.Key}: {item.Value}");
        }
        
        System.Console.WriteLine();
    }

    public void WriteProgressBar(decimal percent)
    {
        const int barLength = 20;
        int filledLength = (int)(barLength * percent / 100);
        string bar = new string('█', filledLength) + new string('░', barLength - filledLength);
        
        System.Console.WriteLine($"  {percent}% |{bar}|");
    }
}
