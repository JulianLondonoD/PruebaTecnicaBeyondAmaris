using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TodoApp.Application.Application.TodoLists;
using TodoApp.Application.Common.Interfaces;
using TodoApp.Domain.Domain.Factories;
using TodoApp.Domain.Domain.TodoLists;
using TodoApp.Infrastructure.Configuration;
using TodoApp.Infrastructure.Data;
using TodoApp.Infrastructure.Data.Entities;
using TodoApp.Infrastructure.Repositories;
using TodoApp.Infrastructure.Resilience;

namespace TodoApp.Tests.Integration;

public class IntegrationTestBase : IDisposable
{
    protected readonly ServiceProvider ServiceProvider;
    private readonly string _dbName;

    public IntegrationTestBase()
    {
        _dbName = Guid.NewGuid().ToString();
        var services = new ServiceCollection();

        services.AddDbContext<TodoDbContext>(options =>
            options.UseInMemoryDatabase(_dbName));

        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(TodoListApplication).Assembly));

        services.AddScoped<ITodoListRepository, TodoListRepository>();
        services.AddScoped<ITodoListDomainService, TodoListDomainService>();
        services.AddScoped<ITodoItemFactory, TodoItemFactory>();
        services.AddScoped<ITodoListApplication, TodoListApplication>();

        // Add resilience services with default options
        services.Configure<ResilienceOptions>(options =>
        {
            options.Retry = new ResilienceOptions.RetryOptions
            {
                MaxRetryAttempts = 3,
                InitialDelaySeconds = 1,
                UseExponentialBackoff = true
            };
            options.CircuitBreaker = new ResilienceOptions.CircuitBreakerOptions
            {
                FailureRatio = 0.5,
                MinimumThroughput = 10,
                SamplingDurationSeconds = 30,
                BreakDurationSeconds = 30
            };
            options.Timeout = new ResilienceOptions.TimeoutOptions
            {
                DefaultTimeoutSeconds = 30,
                DatabaseTimeoutSeconds = 10,
                CommandTimeoutSeconds = 30,
                QueryTimeoutSeconds = 10
            };
        });
        services.AddSingleton<IResiliencePolicyProvider, ResiliencePolicyProvider>();

        // Add logging
        services.AddLogging(builder => builder.AddConsole().SetMinimumLevel(LogLevel.Warning));

        ServiceProvider = services.BuildServiceProvider();
        
        SeedCategories();
    }

    private void SeedCategories()
    {
        using var scope = ServiceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<TodoDbContext>();
        
        dbContext.Categories.AddRange(
            new CategoryEntity { Name = "Work" },
            new CategoryEntity { Name = "Personal" },
            new CategoryEntity { Name = "Study" },
            new CategoryEntity { Name = "Health" }
        );
        dbContext.SaveChanges();
    }

    public void Dispose()
    {
        ServiceProvider?.Dispose();
    }
}
