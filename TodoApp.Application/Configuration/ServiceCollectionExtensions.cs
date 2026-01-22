using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using FluentValidation;
using MediatR;
using TodoApp.Application.Application.TodoLists;
using TodoApp.Application.Behaviors;
using TodoApp.Application.Validators;
using TodoApp.Application.Mappings;

namespace TodoApp.Application.Configuration;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
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
}
