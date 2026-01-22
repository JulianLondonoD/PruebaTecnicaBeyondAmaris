using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Caching.Memory;
using FluentValidation;
using TodoApp.Application.Application.TodoLists;
using TodoApp.Application.Behaviors;
using TodoApp.Application.Common.Interfaces;
using TodoApp.Application.Validators;
using TodoApp.Application.Mappings;
using TodoApp.Console.Console;
using TodoApp.Console.Console.Commands;
using TodoApp.Console.Console.Input;
using TodoApp.Console.Console.Output;
using TodoApp.Console.Configuration.Options;
using TodoApp.Domain.Domain.Aggregates;
using TodoApp.Domain.Domain.Factories;
using TodoApp.Domain.Domain.TodoLists;
using TodoApp.Infrastructure.Adapters.Output;
using TodoApp.Infrastructure.Configuration;
using TodoApp.Infrastructure.Data;
using TodoApp.Infrastructure.Repositories;
using TodoApp.Infrastructure.Caching;
using TodoApp.Infrastructure.HealthChecks;
using TodoApp.Infrastructure.Resilience;
using MediatR;

namespace TodoApp.Configuration;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Configure Options
        services.Configure<DatabaseOptions>(configuration.GetSection(DatabaseOptions.SectionName));
        
        var databaseOptions = configuration.GetSection(DatabaseOptions.SectionName).Get<DatabaseOptions>() 
            ?? new DatabaseOptions();

        services.AddDbContext<TodoDbContext>(options =>
        {
            options.UseSqlite(databaseOptions.ConnectionString);
            
            if (databaseOptions.EnableSensitiveDataLogging)
            {
                options.EnableSensitiveDataLogging();
            }
            
            if (databaseOptions.EnableDetailedErrors)
            {
                options.EnableDetailedErrors();
            }
        });

        services.AddScoped<ITodoListRepository, TodoListRepository>();

        // Output Adapters
        services.AddScoped<ITodoItemOutputAdapter<string>, ConsoleOutputAdapter>();
        services.AddKeyedScoped<ITodoItemOutputAdapter<string>, JsonOutputAdapter>("json");
        services.AddKeyedScoped<ITodoItemOutputAdapter<IEnumerable<object>>, WebOutputAdapter>("web");

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

    public static IServiceCollection AddDomain(this IServiceCollection services)
    {
        services.AddScoped<ITodoListDomainService, TodoListDomainService>();
        services.AddScoped<ITodoItemFactory, TodoItemFactory>();

        return services;
    }

    public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        // Configure Options
        services.Configure<BusinessOptions>(configuration.GetSection(BusinessOptions.SectionName));
        services.Configure<LoggingOptions>(configuration.GetSection(LoggingOptions.SectionName));
        services.Configure<CachingOptions>(configuration.GetSection(CachingOptions.SectionName));

        // MediatR with Behaviors
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(TodoListApplication).Assembly);
            
            // Add pipeline behaviors (order matters!)
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ExceptionHandlingBehavior<,>));
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ResilienceBehavior<,>));
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(PerformanceBehavior<,>));
        });

        services.AddScoped<ITodoListApplication, TodoListApplication>();

        // FluentValidation
        services.AddValidatorsFromAssemblyContaining<AddTodoItemCommandValidator>();

        // AutoMapper
        services.AddAutoMapper(typeof(TodoItemProfile).Assembly);

        return services;
    }

    public static IServiceCollection AddConsole(this IServiceCollection services, IConfiguration configuration)
    {
        // Configure Options
        services.Configure<ApplicationOptions>(configuration.GetSection(ApplicationOptions.SectionName));

        // Console infrastructure
        services.AddSingleton<IConsoleWriter, ConsoleWriter>();
        services.AddSingleton<IInputValidator, CommandInputValidator>();
        services.AddSingleton<IInteractiveInput, InteractiveInput>();

        // Command handlers
        services.AddScoped<ICommandHandler, AddTodoCommandHandler>();
        services.AddScoped<ICommandHandler, UpdateTodoCommandHandler>();
        services.AddScoped<ICommandHandler, RemoveTodoCommandHandler>();
        services.AddScoped<ICommandHandler, ProgressCommandHandler>();
        services.AddScoped<ICommandHandler, ListCommandHandler>();
        services.AddScoped<ICommandHandler, CategoriesCommandHandler>();
        services.AddScoped<ICommandHandler, HelpCommandHandler>();
        services.AddScoped<ICommandHandler, ExitCommandHandler>();

        // Command dispatcher
        services.AddScoped<ICommandDispatcher, CommandDispatcher>();

        // Console application
        services.AddScoped<IConsoleApplication, ConsoleApplication>();

        return services;
    }
}
