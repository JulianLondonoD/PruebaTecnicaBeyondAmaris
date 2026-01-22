using TodoApp.Console.Console.Output;

namespace TodoApp.Console.Console.Commands;

public class ExitCommandHandler : ICommandHandler
{
    private readonly IConsoleWriter _writer;

    public string CommandName => "exit";
    public string Description => "Salir de la aplicación";

    public ExitCommandHandler(IConsoleWriter writer)
    {
        _writer = writer ?? throw new ArgumentNullException(nameof(writer));
    }

    public Task<bool> CanHandle(string command)
    {
        return Task.FromResult(command.Equals(CommandName, StringComparison.OrdinalIgnoreCase));
    }

    public Task<bool> HandleAsync(string[] args)
    {
        _writer.WriteInfo("¡Hasta luego!");
        return Task.FromResult(false); // Signal to exit the application
    }
}
