using TodoApp.Application.Application.TodoLists;
using TodoApp.Console.Console.Input;
using TodoApp.Console.Console.Output;

namespace TodoApp.Console.Console.Commands;

public class UpdateTodoCommandHandler : ICommandHandler
{
    private readonly ITodoListApplication _app;
    private readonly IInteractiveInput _interactiveInput;
    private readonly IConsoleWriter _writer;

    public string CommandName => "update";
    public string Description => "Actualizar descripción de tarea";

    public UpdateTodoCommandHandler(
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
            _interactiveInput.ShowHeader("Actualizar tarea");

            var id = await _interactiveInput.ReadIntAsync("Ingresa el ID de la tarea:", min: 1);
            
            var newDescription = await _interactiveInput.ReadTextAsync("Ingresa la nueva descripción:");

            var preview = new Dictionary<string, string>
            {
                { "ID de tarea", id.ToString() },
                { "Nueva descripción", newDescription }
            };
            _writer.WritePreview("Cambios:", preview);

            var confirmed = await _interactiveInput.ReadConfirmationAsync("¿Confirmas la actualización?");
            
            if (!confirmed)
            {
                _writer.WriteWarning("Actualización cancelada.");
                return true;
            }

            await _app.UpdateItemAsync(id, newDescription);
            _writer.WriteSuccess("✨ Tarea actualizada exitosamente");
        }
        catch (Exception ex)
        {
            _writer.WriteError($"Error: {ex.Message}");
        }

        return true;
    }
}
