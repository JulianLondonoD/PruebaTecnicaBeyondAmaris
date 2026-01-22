using TodoApp.Application.Application.TodoLists;
using TodoApp.Console.Console.Input;
using TodoApp.Console.Console.Output;

namespace TodoApp.Console.Console.Commands;

public class ProgressCommandHandler : ICommandHandler
{
    private readonly ITodoListApplication _app;
    private readonly IInteractiveInput _interactiveInput;
    private readonly IConsoleWriter _writer;

    public string CommandName => "progress";
    public string Description => "Registrar progreso";

    public ProgressCommandHandler(
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
            _interactiveInput.ShowHeader("Registrar progreso");

            var id = await _interactiveInput.ReadIntAsync("Ingresa el ID de la tarea:", min: 1);
            
            var percent = await _interactiveInput.ReadDecimalAsync("Porcentaje de progreso (1-100):", min: 1, max: 100);

            _writer.WriteInfo("Vista previa:");
            _writer.WriteProgressBar(percent);

            var confirmed = await _interactiveInput.ReadConfirmationAsync("¿Confirmar registro de progreso?");
            
            if (!confirmed)
            {
                _writer.WriteWarning("Registro de progreso cancelado.");
                return true;
            }

            await _app.RegisterProgressionAsync(id, DateTime.Now, percent);
            _writer.WriteSuccess("✨ Progreso registrado exitosamente");
        }
        catch (Exception ex)
        {
            _writer.WriteError($"Error: {ex.Message}");
        }

        return true;
    }
}
