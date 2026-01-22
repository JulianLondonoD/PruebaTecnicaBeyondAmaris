using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TodoApp.Console.Console.Commands;
using TodoApp.Console.Console.Output;
using TodoApp.Infrastructure.Data;

namespace TodoApp.Console.Console;

public class ConsoleApplication : IConsoleApplication
{
    private readonly ICommandDispatcher _dispatcher;
    private readonly IConsoleWriter _writer;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<ConsoleApplication> _logger;

    public ConsoleApplication(
        ICommandDispatcher dispatcher,
        IConsoleWriter writer,
        IServiceProvider serviceProvider,
        ILogger<ConsoleApplication> logger)
    {
        _dispatcher = dispatcher ?? throw new ArgumentNullException(nameof(dispatcher));
        _writer = writer ?? throw new ArgumentNullException(nameof(writer));
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task RunAsync()
    {
        try
        {
            // Initialize database
            await InitializeDatabaseAsync();

            // Display header
            _writer.WriteHeader();
            _writer.WriteInfo("Escribe 'help' para ver los comandos disponibles");
            _writer.WriteLine();

            // Main loop
            var continueRunning = true;
            while (continueRunning)
            {
                try
                {
                    _writer.Write("> ");
                    var input = System.Console.ReadLine();

                    continueRunning = await _dispatcher.DispatchAsync(input ?? string.Empty);
                    _writer.WriteLine();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing command");
                    _writer.WriteError($"Error: {ex.Message}");
                    _writer.WriteLine();
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "Fatal error in console application");
            _writer.WriteError($"Error fatal: {ex.Message}");
            throw;
        }
    }

    private async Task InitializeDatabaseAsync()
    {
        try
        {
            using var scope = _serviceProvider.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<TodoDbContext>();
            await db.Database.EnsureCreatedAsync();
            _logger.LogInformation("Database initialized successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error initializing database");
            throw;
        }
    }
}
