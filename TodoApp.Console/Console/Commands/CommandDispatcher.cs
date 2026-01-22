using TodoApp.Console.Console.Output;

namespace TodoApp.Console.Console.Commands;

public class CommandDispatcher : ICommandDispatcher
{
    private readonly IEnumerable<ICommandHandler> _handlers;
    private readonly IConsoleWriter _writer;

    public CommandDispatcher(IEnumerable<ICommandHandler> handlers, IConsoleWriter writer)
    {
        _handlers = handlers ?? throw new ArgumentNullException(nameof(handlers));
        _writer = writer ?? throw new ArgumentNullException(nameof(writer));
    }

    public async Task<bool> DispatchAsync(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            return true;
        }

        var parts = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var command = parts[0].ToLower();

        foreach (var handler in _handlers)
        {
            if (await handler.CanHandle(command))
            {
                return await handler.HandleAsync(parts);
            }
        }

        _writer.WriteError($"Comando desconocido: '{command}'. Escribe 'help' para ver los comandos disponibles.");
        return true;
    }

    public IEnumerable<ICommandHandler> GetAllHandlers()
    {
        return _handlers;
    }
}
