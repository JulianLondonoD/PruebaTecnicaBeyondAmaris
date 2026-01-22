using TodoApp.Console.Console.Output;

namespace TodoApp.Console.Console.Input;

public class InteractiveInput : IInteractiveInput
{
    private readonly IConsoleWriter _writer;

    public InteractiveInput(IConsoleWriter writer)
    {
        _writer = writer ?? throw new ArgumentNullException(nameof(writer));
    }

    public void ShowHeader(string title)
    {
        _writer.WriteLine();
        System.Console.ForegroundColor = ConsoleColor.Cyan;
        System.Console.WriteLine($"📝 {title}");
        System.Console.ResetColor();
    }

    public void ShowSeparator()
    {
        _writer.WriteLine();
    }

    public Task<string> ReadTextAsync(string prompt, bool required = true)
    {
        while (true)
        {
            _writer.WriteBox(prompt);
            System.Console.Write("> ");
            var input = System.Console.ReadLine();
            
            if (string.IsNullOrWhiteSpace(input))
            {
                if (required)
                {
                    _writer.WriteError("Este campo es obligatorio. Por favor ingresa un valor.");
                    continue;
                }
                return Task.FromResult(string.Empty);
            }
            
            return Task.FromResult(input.Trim());
        }
    }

    public Task<int> ReadIntAsync(string prompt, int? min = null, int? max = null)
    {
        while (true)
        {
            _writer.WriteBox(prompt);
            System.Console.Write("> ");
            var input = System.Console.ReadLine();
            
            if (!int.TryParse(input, out int value))
            {
                _writer.WriteError("Por favor ingresa un número válido.");
                continue;
            }
            
            if (min.HasValue && value < min.Value)
            {
                _writer.WriteError($"El valor debe ser mayor o igual a {min.Value}.");
                continue;
            }
            
            if (max.HasValue && value > max.Value)
            {
                _writer.WriteError($"El valor debe ser menor o igual a {max.Value}.");
                continue;
            }
            
            return Task.FromResult(value);
        }
    }

    public Task<decimal> ReadDecimalAsync(string prompt, decimal? min = null, decimal? max = null)
    {
        while (true)
        {
            _writer.WriteBox(prompt);
            System.Console.Write("> ");
            var input = System.Console.ReadLine();
            
            if (!decimal.TryParse(input, out decimal value))
            {
                _writer.WriteError("Por favor ingresa un número válido.");
                continue;
            }
            
            if (min.HasValue && value < min.Value)
            {
                _writer.WriteError($"El valor debe ser mayor o igual a {min.Value}.");
                continue;
            }
            
            if (max.HasValue && value > max.Value)
            {
                _writer.WriteError($"El valor debe ser menor o igual a {max.Value}.");
                continue;
            }
            
            return Task.FromResult(value);
        }
    }

    public Task<int> ReadOptionAsync(string prompt, List<string> options)
    {
        while (true)
        {
            System.Console.WriteLine("┌─────────────────────────────────────┐");
            System.Console.WriteLine($"│ {prompt.PadRight(36)}│");
            
            for (int i = 0; i < options.Count; i++)
            {
                System.Console.WriteLine($"│ {(i + 1)}) {options[i].PadRight(34)}│");
            }
            
            System.Console.WriteLine("└─────────────────────────────────────┘");
            System.Console.Write("> ");
            
            var input = System.Console.ReadLine();
            
            if (!int.TryParse(input, out int selection))
            {
                _writer.WriteError("Por favor ingresa un número válido.");
                continue;
            }
            
            if (selection < 1 || selection > options.Count)
            {
                _writer.WriteError($"Por favor selecciona una opción entre 1 y {options.Count}.");
                continue;
            }
            
            return Task.FromResult(selection);
        }
    }

    public Task<bool> ReadConfirmationAsync(string prompt)
    {
        _writer.WriteBox($"{prompt} (s/n)");
        System.Console.Write("> ");
        var input = System.Console.ReadLine();
        
        return Task.FromResult(input?.Trim().ToLower() == "s");
    }
}
