using Microsoft.Extensions.DependencyInjection;
using TodoApp.Domain.Domain.TodoLists;
using TodoApp.Domain.Domain.Factories;

namespace TodoApp.Domain.Configuration;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDomain(this IServiceCollection services)
    {
        services.AddScoped<ITodoListDomainService, TodoListDomainService>();
        services.AddScoped<ITodoItemFactory, TodoItemFactory>();

        return services;
    }
}
