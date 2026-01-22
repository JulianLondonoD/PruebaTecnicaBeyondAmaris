using TodoApp.Application.Application.TodoLists;
using TodoApp.Console.Console.Input;
using TodoApp.Console.Console.Output;

namespace TodoApp.Console.Console.Commands;

public class AddTodoCommandHandler : ICommandHandler
{
    private readonly ITodoListApplication _app;
    private readonly IInteractiveInput _interactiveInput;
    private readonly IConsoleWriter _writer;

    public string CommandName => "add";
    public string Description => "Añadir nueva tarea";

    public AddTodoCommandHandler(
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
            _interactiveInput.ShowHeader("Crear nueva tarea");

            var title = await _interactiveInput.ReadTextAsync("Ingresa el título de la tarea:");
            
            var description = await _interactiveInput.ReadTextAsync("Ingresa la descripción:");
            
            var categories = await _app.GetCategoriesAsync();
            var categoryIndex = await _interactiveInput.ReadOptionAsync("Selecciona una categoría:", categories);
            var category = categories[categoryIndex - 1];

            var preview = new Dictionary<string, string>
            {
                { "Título", title },
                { "Descripción", description },
                { "Categoría", category }
            };
            _writer.WritePreview("Resumen de la nueva tarea:", preview);

            var confirmed = await _interactiveInput.ReadConfirmationAsync("¿Confirmas la creación de esta tarea?");
            
            if (!confirmed)
            {
                _writer.WriteWarning("Creación de tarea cancelada.");
                return true;
            }

            var id = await _app.GetNextIdAsync();
            await _app.AddItemAsync(id, title, description, category);
            
            _writer.WriteSuccess($"✨ Tarea \"{title}\" creada exitosamente con ID: {id}");
        }
        catch (Exception ex)
        {
            _writer.WriteError($"Error: {ex.Message}");
        }

        return true;
    }
}
