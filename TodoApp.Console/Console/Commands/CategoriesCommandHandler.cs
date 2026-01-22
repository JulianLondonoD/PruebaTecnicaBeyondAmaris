using TodoApp.Application.Application.TodoLists;
using TodoApp.Console.Console.Output;

namespace TodoApp.Console.Console.Commands;

public class CategoriesCommandHandler : ICommandHandler
{
    private readonly ITodoListApplication _app;
    private readonly IConsoleWriter _writer;

    public string CommandName => "categories";
    public string Description => "Mostrar todas las categorías";

    public CategoriesCommandHandler(
        ITodoListApplication app,
        IConsoleWriter writer)
    {
        _app = app ?? throw new ArgumentNullException(nameof(app));
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
            var categories = await _app.GetCategoriesAsync();
            _writer.WriteInfo("Categorías disponibles:");
            foreach (var category in categories)
            {
                _writer.WriteLine($"  - {category}");
            }
        }
        catch (Exception ex)
        {
            _writer.WriteError($"Error: {ex.Message}");
        }

        return true;
    }
}
