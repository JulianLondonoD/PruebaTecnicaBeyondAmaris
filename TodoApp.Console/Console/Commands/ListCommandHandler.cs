using MediatR;
using TodoApp.Application.Application.Queries;
using TodoApp.Console.Console.Output;
using TodoApp.Infrastructure.Adapters.Output;

namespace TodoApp.Console.Console.Commands;

public class ListCommandHandler : ICommandHandler
{
    private readonly IMediator _mediator;
    private readonly IConsoleWriter _consoleWriter;
    private readonly ITodoItemOutputAdapter<string> _outputAdapter;

    public string CommandName => "list";
    public string Description => "Mostrar todas las tareas";

    public ListCommandHandler(
        IMediator mediator,
        IConsoleWriter consoleWriter,
        ITodoItemOutputAdapter<string> outputAdapter)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _consoleWriter = consoleWriter ?? throw new ArgumentNullException(nameof(consoleWriter));
        _outputAdapter = outputAdapter ?? throw new ArgumentNullException(nameof(outputAdapter));
    }

    public Task<bool> CanHandle(string command)
    {
        return Task.FromResult(command.Equals(CommandName, StringComparison.OrdinalIgnoreCase));
    }

    public async Task<bool> HandleAsync(string[] args)
    {
        try
        {
            // 1. Obtener datos del domain (vía Application)
            var items = await _mediator.Send(new GetTodoItemsQuery());
            
            // 2. Formatear usando Infrastructure adapter
            var formattedOutput = _outputAdapter.FormatItems(items);
            
            // 3. Mostrar usando Presentation writer
            if (string.IsNullOrEmpty(formattedOutput))
            {
                _consoleWriter.WriteInfo("No hay tareas registradas.");
            }
            else
            {
                _consoleWriter.WriteLine(formattedOutput);
            }
        }
        catch (Exception ex)
        {
            _consoleWriter.WriteError($"Error al mostrar las tareas: {ex.Message}");
        }
        
        return true;
    }
}
