using Microsoft.Extensions.DependencyInjection;
using TodoApp.Application.Application.TodoLists;

namespace TodoApp.Tests.Integration;

public class TodoAppIntegrationTests : IntegrationTestBase
{
    [Fact]
    public async Task FullWorkflow_AddUpdateProgressRemove_Success()
    {
        using var scope = ServiceProvider.CreateScope();
        var app = scope.ServiceProvider.GetRequiredService<ITodoListApplication>();

        var id = await app.GetNextIdAsync();
        await app.AddItemAsync(id, "Test Task", "Test Description", "Work");

        await app.UpdateItemAsync(id, "Updated Description");

        await app.RegisterProgressionAsync(id, DateTime.Now, 30);

        await app.RemoveItemAsync(id);
    }

    [Fact]
    public async Task AddMultipleItems_AndRegisterProgress_Success()
    {
        using var scope = ServiceProvider.CreateScope();
        var app = scope.ServiceProvider.GetRequiredService<ITodoListApplication>();

        var id1 = await app.GetNextIdAsync();
        await app.AddItemAsync(id1, "Task 1", "Description 1", "Work");

        var id2 = await app.GetNextIdAsync();
        await app.AddItemAsync(id2, "Task 2", "Description 2", "Personal");

        await app.RegisterProgressionAsync(id1, DateTime.Now, 50);
        await app.RegisterProgressionAsync(id2, DateTime.Now, 100);
    }

    [Fact]
    public async Task GetCategories_ReturnsSeededCategories()
    {
        using var scope = ServiceProvider.CreateScope();
        var app = scope.ServiceProvider.GetRequiredService<ITodoListApplication>();

        var categories = await app.GetCategoriesAsync();

        Assert.Equal(4, categories.Count);
        Assert.Contains("Work", categories);
        Assert.Contains("Personal", categories);
        Assert.Contains("Study", categories);
        Assert.Contains("Health", categories);
    }

    [Fact]
    public async Task RegisterProgression_ExceedsTotal_ThrowsException()
    {
        using var scope = ServiceProvider.CreateScope();
        var app = scope.ServiceProvider.GetRequiredService<ITodoListApplication>();

        var id = await app.GetNextIdAsync();
        await app.AddItemAsync(id, "Test Task", "Test Description", "Work");
        await app.RegisterProgressionAsync(id, DateTime.Now, 60);

        await Assert.ThrowsAsync<TodoApp.Domain.Domain.Exceptions.ProgressionPercentException>(async () =>
            await app.RegisterProgressionAsync(id, DateTime.Now.AddDays(1), 50));
    }

    [Fact]
    public async Task UpdateItem_ProgressOver50_ThrowsException()
    {
        using var scope = ServiceProvider.CreateScope();
        var app = scope.ServiceProvider.GetRequiredService<ITodoListApplication>();

        var id = await app.GetNextIdAsync();
        await app.AddItemAsync(id, "Test Task", "Test Description", "Work");
        await app.RegisterProgressionAsync(id, DateTime.Now, 60);

        await Assert.ThrowsAsync<TodoApp.Domain.Domain.Exceptions.TodoItemProgressException>(async () =>
            await app.UpdateItemAsync(id, "Updated Description"));
    }

    [Fact]
    public async Task RemoveItem_ProgressOver50_ThrowsException()
    {
        using var scope = ServiceProvider.CreateScope();
        var app = scope.ServiceProvider.GetRequiredService<ITodoListApplication>();

        var id = await app.GetNextIdAsync();
        await app.AddItemAsync(id, "Test Task", "Test Description", "Work");
        await app.RegisterProgressionAsync(id, DateTime.Now, 60);

        await Assert.ThrowsAsync<TodoApp.Domain.Domain.Exceptions.TodoItemProgressException>(async () =>
            await app.RemoveItemAsync(id));
    }
}
