using TodoApp.Application.Application.TodoLists;
using TodoApp.Console.Console.Input;
using TodoApp.Console.Console.Output;

namespace TodoApp.Console.Console.Commands;

public class RemoveTodoCommandHandler : ICommandHandler
{
    private readonly ITodoListApplication _app;
    private readonly IInteractiveInput _interactiveInput;
    private readonly IConsoleWriter _writer;

    public string CommandName => "remove";
    public string Description => "Eliminar tarea";

    public RemoveTodoCommandHandler(
        ITodoListApplication app,
        IInteractiveInput interactiveInput,
        IConsoleWriter writer)
    {
        _app = app ?? throw new ArgumentNullException(nameof(app));
        _interactiveInput = interactiveInput ?? throw new ArgumentNullException(nameof(interactiveInput));
        _writer = writer ?? throw new ArgumentNullException(nameof(writer));
    }

    public Task<bool> CanHandle(string command)
    {
        return Task.FromResult(command.Equals(CommandName, StringComparison.OrdinalIgnoreCase));
    }

    public async Task<bool> HandleAsync(string[] args)
    {
        try
        {
            _interactiveInput.ShowHeader("Eliminar tarea");

            var id = await _interactiveInput.ReadIntAsync("Ingresa el ID de la tarea:", min: 1);

            _writer.WriteWarning("⚠️ ADVERTENCIA: Esta acción es irreversible");
            
            var confirmed = await _interactiveInput.ReadConfirmationAsync("¿Estás seguro de eliminar la tarea?");
            
            if (!confirmed)
            {
                _writer.WriteWarning("Eliminación cancelada.");
                return true;
            }

            await _app.RemoveItemAsync(id);
            _writer.WriteSuccess("✨ Tarea eliminada exitosamente");
        }
        catch (Exception ex)
        {
            _writer.WriteError($"Error: {ex.Message}");
        }

        return true;
    }
}
