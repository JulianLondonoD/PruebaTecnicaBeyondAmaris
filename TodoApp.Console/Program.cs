using Microsoft.Extensions.DependencyInjection;
using Serilog;
using TodoApp.Configuration;
using TodoApp.Console.Console;

try
{
    var host = TodoAppHostBuilder.Create();
    
    Log.Information("Aplicación iniciada correctamente");
    
    using var scope = host.Services.CreateScope();
    var app = scope.ServiceProvider.GetRequiredService<IConsoleApplication>();
    await app.RunAsync();
    
    Log.Information("Aplicación finalizada correctamente");
}
catch (Exception ex)
{
    Log.Fatal(ex, "Error fatal en la aplicación");
    throw;
}
finally
{
    Log.CloseAndFlush();
}
