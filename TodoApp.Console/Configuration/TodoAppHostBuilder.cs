using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using TodoApp.Console.Logging;

namespace TodoApp.Configuration;

public static class TodoAppHostBuilder
{
    public static IHost Create()
    {
        // Create bootstrap logger for startup
        Log.Logger = SerilogConfiguration.CreateBootstrapLogger();

        try
        {
            Log.Information("Iniciando TodoApp...");

            var hostBuilder = Host.CreateDefaultBuilder();
            
            // ✅ NUEVO: Detectar environment variables
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") 
                           ?? Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT");
            
            // ✅ NUEVO: Configurar Development por defecto si no hay variable
            if (string.IsNullOrEmpty(environment))
            {
                hostBuilder.UseEnvironment("Development");
                Log.Information("No se encontró variable de entorno, usando Development por defecto");
            }
            else
            {
                hostBuilder.UseEnvironment(environment);
                Log.Information("Usando environment de variable de entorno: {Environment}", environment);
            }

            return hostBuilder
                .ConfigureAppConfiguration((context, config) =>
                {
                    config.SetBasePath(Directory.GetCurrentDirectory());
                    config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                    
                    // ✅ NUEVO: Log del archivo de configuración que se carga
                    var configFile = $"appsettings.{context.HostingEnvironment.EnvironmentName}.json";
                    Log.Information("Cargando archivo de configuración: {ConfigFile}", configFile);
                    
                    config.AddJsonFile(configFile, optional: true, reloadOnChange: true);
                    config.AddEnvironmentVariables();
                    
                    // ✅ NUEVO: Confirmación del environment activo
                    Log.Information("Environment activo: {Environment}", context.HostingEnvironment.EnvironmentName);
                })
                .UseSerilog((context, services, configuration) =>
                {
                    // Replace bootstrap logger with configured logger
                    Log.Logger = SerilogConfiguration.CreateLogger(context.Configuration);
                })
                .ConfigureServices((context, services) =>
                {
                    services.AddInfrastructure(context.Configuration);
                    services.AddDomain();
                    services.AddApplication(context.Configuration);
                    services.AddConsole(context.Configuration);
                })
                .Build();
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Error fatal al iniciar la aplicación");
            throw;
        }
    }
}
