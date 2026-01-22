using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TodoApp.Application.Application.TodoLists;
using TodoApp.Infrastructure.Data;
using Xunit;

namespace TodoApp.Tests.Integration;

public class DeleteEndpointTests : IntegrationTestBase
{
    [Fact]
    public async Task DeleteTodoItem_RemovesFromDatabase_Successfully()
    {
        using var scope = ServiceProvider.CreateScope();
        var app = scope.ServiceProvider.GetRequiredService<ITodoListApplication>();
        var context = scope.ServiceProvider.GetRequiredService<TodoDbContext>();

        // Arrange: Create a new todo item with progress < 50%
        var id = await app.GetNextIdAsync();
        await app.AddItemAsync(id, "Test Delete Item", "Testing DELETE fix", "Work");
        await app.RegisterProgressionAsync(id, DateTime.Now, 30);

        // Verify item exists in database before deletion
        var itemBeforeDelete = await context.TodoItems.FindAsync(id);
        Assert.NotNull(itemBeforeDelete);
        Assert.Equal("Test Delete Item", itemBeforeDelete.Title);

        // Act: Delete the item
        await app.RemoveItemAsync(id);

        // Assert: Verify item is deleted from database
        var itemAfterDelete = await context.TodoItems.FindAsync(id);
        Assert.Null(itemAfterDelete);

        // Verify progressions are also deleted (CASCADE)
        var progressions = await context.Progressions.Where(p => p.TodoItemId == id).ToListAsync();
        Assert.Empty(progressions);
    }

    [Fact]
    public async Task DeleteTodoItem_WithMultipleProgressions_RemovesAllData()
    {
        using var scope = ServiceProvider.CreateScope();
        var app = scope.ServiceProvider.GetRequiredService<ITodoListApplication>();
        var context = scope.ServiceProvider.GetRequiredService<TodoDbContext>();

        // Arrange: Create item with multiple progressions
        var id = await app.GetNextIdAsync();
        await app.AddItemAsync(id, "Item with Progressions", "Multiple progressions", "Personal");
        await app.RegisterProgressionAsync(id, DateTime.Now, 10);
        await app.RegisterProgressionAsync(id, DateTime.Now.AddHours(1), 15);
        await app.RegisterProgressionAsync(id, DateTime.Now.AddHours(2), 20);

        // Verify progressions exist
        var progressionsBeforeDelete = await context.Progressions.Where(p => p.TodoItemId == id).ToListAsync();
        Assert.Equal(3, progressionsBeforeDelete.Count);

        // Act: Delete the item
        await app.RemoveItemAsync(id);

        // Assert: Verify all data is deleted
        var itemAfterDelete = await context.TodoItems.FindAsync(id);
        Assert.Null(itemAfterDelete);

        var progressionsAfterDelete = await context.Progressions.Where(p => p.TodoItemId == id).ToListAsync();
        Assert.Empty(progressionsAfterDelete);
    }

    [Fact]
    public async Task DeleteTodoItem_LeavesOtherItems_Intact()
    {
        using var scope = ServiceProvider.CreateScope();
        var app = scope.ServiceProvider.GetRequiredService<ITodoListApplication>();
        var context = scope.ServiceProvider.GetRequiredService<TodoDbContext>();

        // Arrange: Create two items
        var id1 = await app.GetNextIdAsync();
        await app.AddItemAsync(id1, "Item 1", "First item", "Work");
        await app.RegisterProgressionAsync(id1, DateTime.Now, 25);

        var id2 = await app.GetNextIdAsync();
        await app.AddItemAsync(id2, "Item 2", "Second item", "Personal");
        await app.RegisterProgressionAsync(id2, DateTime.Now, 30);

        // Act: Delete only the first item
        await app.RemoveItemAsync(id1);

        // Assert: Verify first item is deleted
        var item1AfterDelete = await context.TodoItems.FindAsync(id1);
        Assert.Null(item1AfterDelete);

        // Verify second item still exists
        var item2AfterDelete = await context.TodoItems.FindAsync(id2);
        Assert.NotNull(item2AfterDelete);
        Assert.Equal("Item 2", item2AfterDelete.Title);

        // Verify second item's progressions still exist
        var progressions2 = await context.Progressions.Where(p => p.TodoItemId == id2).ToListAsync();
        Assert.Single(progressions2);
    }
}
