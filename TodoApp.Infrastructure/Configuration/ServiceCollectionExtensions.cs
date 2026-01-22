using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Caching.Memory;
using TodoApp.Application.Common.Interfaces;
using TodoApp.Infrastructure.Data;
using TodoApp.Infrastructure.Repositories;
using TodoApp.Infrastructure.Caching;
using TodoApp.Infrastructure.HealthChecks;
using TodoApp.Infrastructure.Resilience;
using TodoApp.Domain.Domain.TodoLists;

namespace TodoApp.Infrastructure.Configuration;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Database
        services.AddDbContext<TodoDbContext>(options =>
        {
            var connectionString = configuration.GetSection("TodoApp:Database:ConnectionString").Value
                ?? configuration.GetConnectionString("DefaultConnection") 
                ?? "Data Source=todoapp.db";
            options.UseSqlite(connectionString);
        });

        services.AddScoped<ITodoListRepository, TodoListRepository>();

        // Caching
        services.AddMemoryCache();
        services.AddSingleton<ICacheService, MemoryCacheService>();

        // Health Checks
        services.AddHealthChecks()
            .AddCheck<DatabaseHealthCheck>("database")
            .AddCheck<BusinessRulesHealthCheck>("business-rules");

        // Resilience Policies
        var resilienceSection = configuration.GetSection(ResilienceOptions.SectionName);
        services.Configure<ResilienceOptions>(resilienceSection);
        services.AddSingleton<IResiliencePolicyProvider, ResiliencePolicyProvider>();

        return services;
    }
}
