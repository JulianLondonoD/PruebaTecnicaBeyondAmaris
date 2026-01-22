using Microsoft.Extensions.DependencyInjection;
using TodoApp.Console.Console.Output;

namespace TodoApp.Console.Console.Commands;

public class HelpCommandHandler : ICommandHandler
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IConsoleWriter _writer;

    public string CommandName => "help";
    public string Description => "Mostrar ayuda";

    public HelpCommandHandler(
        IServiceProvider serviceProvider,
        IConsoleWriter writer)
    {
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        _writer = writer ?? throw new ArgumentNullException(nameof(writer));
    }

    public Task<bool> CanHandle(string command)
    {
        return Task.FromResult(command.Equals(CommandName, StringComparison.OrdinalIgnoreCase));
    }

    public Task<bool> HandleAsync(string[] args)
    {
        _writer.WriteInfo("Comandos disponibles:");
        
        // Resolve handlers lazily to avoid circular dependency
        var handlers = _serviceProvider.GetServices<ICommandHandler>().OrderBy(h => h.CommandName);
        
        foreach (var handler in handlers)
        {
            _writer.WriteLine($"  {handler.CommandName,-12} - {handler.Description}");
        }

        return Task.FromResult(true);
    }
}
