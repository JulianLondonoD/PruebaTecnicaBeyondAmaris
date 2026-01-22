using Microsoft.Extensions.DependencyInjection;
using TodoApp.Application.Application.Queries;
using TodoApp.Infrastructure.Adapters.Output;
using TodoApp.Domain.Domain.ValueObjects;
using MediatR;

namespace TodoApp.Tests.Integration;

public class GetTodoItemsIntegrationTests : IntegrationTestBase
{
    [Fact]
    public async Task GetTodoItems_WithNoItems_ReturnsEmptyCollection()
    {
        using var scope = ServiceProvider.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        var items = await mediator.Send(new GetTodoItemsQuery());

        Assert.Empty(items);
    }

    [Fact]
    public async Task GetTodoItems_WithItems_ReturnsAllItems()
    {
        using var scope = ServiceProvider.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var app = scope.ServiceProvider.GetRequiredService<TodoApp.Application.Application.TodoLists.ITodoListApplication>();

        // Add items
        var id1 = await app.GetNextIdAsync();
        await app.AddItemAsync(id1, "Task 1", "Description 1", "Work");

        var id2 = await app.GetNextIdAsync();
        await app.AddItemAsync(id2, "Task 2", "Description 2", "Personal");

        // Get items
        var items = await mediator.Send(new GetTodoItemsQuery());

        Assert.Equal(2, items.Count());
        
        var firstItem = items.First();
        Assert.Equal(id1, firstItem.Id);
        Assert.Equal("Task 1", firstItem.Title);
        Assert.Equal("Description 1", firstItem.Description);
        Assert.Equal("Work", firstItem.Category);
        Assert.False(firstItem.IsCompleted);
        Assert.Equal(0, firstItem.TotalProgress);
    }

    [Fact]
    public async Task GetTodoItems_WithProgressions_ReturnsItemsWithProgressions()
    {
        using var scope = ServiceProvider.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var app = scope.ServiceProvider.GetRequiredService<TodoApp.Application.Application.TodoLists.ITodoListApplication>();

        // Add item
        var id = await app.GetNextIdAsync();
        await app.AddItemAsync(id, "Task with Progress", "Test Description", "Work");

        // Add progressions
        var now = DateTime.Now;
        await app.RegisterProgressionAsync(id, now, 30);
        await app.RegisterProgressionAsync(id, now.AddDays(1), 40);
        await app.RegisterProgressionAsync(id, now.AddDays(2), 30);

        // Get items
        var items = await mediator.Send(new GetTodoItemsQuery());

        Assert.Single(items);
        var item = items.First();
        
        Assert.Equal(100, item.TotalProgress);
        Assert.True(item.IsCompleted);
        
        var progressions = item.Progressions.ToList();
        Assert.Equal(3, progressions.Count);
        
        Assert.Equal(30, progressions[0].Percent);
        Assert.Equal(30, progressions[0].AccumulatedPercent);
        
        Assert.Equal(40, progressions[1].Percent);
        Assert.Equal(70, progressions[1].AccumulatedPercent);
        
        Assert.Equal(30, progressions[2].Percent);
        Assert.Equal(100, progressions[2].AccumulatedPercent);
    }

    [Fact]
    public void ConsoleOutputAdapter_WithItems_FormatsCorrectly()
    {
        var adapter = new ConsoleOutputAdapter();
        
        var items = new[]
        {
            new TodoItemView
            {
                Id = 1,
                Title = "Test Task",
                Description = "Test Description",
                Category = "Work",
                IsCompleted = false,
                TotalProgress = 50,
                Progressions = new[]
                {
                    new ProgressionView
                    {
                        DateTime = new DateTime(2026, 1, 21, 10, 0, 0),
                        Percent = 30,
                        AccumulatedPercent = 30
                    },
                    new ProgressionView
                    {
                        DateTime = new DateTime(2026, 1, 22, 10, 0, 0),
                        Percent = 20,
                        AccumulatedPercent = 50
                    }
                }
            }
        };

        var output = adapter.FormatItems(items);

        Assert.Contains("1) Test Task - Test Description (Work) Completed:False", output);
        Assert.Contains("30%", output);
        Assert.Contains("50%", output);
        Assert.Contains("|", output); // Progress bar
    }

    [Fact]
    public void JsonOutputAdapter_WithItems_FormatsCorrectly()
    {
        var adapter = new JsonOutputAdapter();
        
        var items = new[]
        {
            new TodoItemView
            {
                Id = 1,
                Title = "Test Task",
                Description = "Test Description",
                Category = "Work",
                IsCompleted = false,
                TotalProgress = 50,
                Progressions = new[]
                {
                    new ProgressionView
                    {
                        DateTime = new DateTime(2026, 1, 21, 10, 0, 0),
                        Percent = 30,
                        AccumulatedPercent = 30
                    }
                }
            }
        };

        var output = adapter.FormatItems(items);

        Assert.Contains("\"Id\": 1", output);
        Assert.Contains("\"Title\": \"Test Task\"", output);
        Assert.Contains("\"Category\": \"Work\"", output);
    }

    [Fact]
    public void WebOutputAdapter_WithItems_FormatsCorrectly()
    {
        var adapter = new WebOutputAdapter();
        
        var items = new[]
        {
            new TodoItemView
            {
                Id = 1,
                Title = "Test Task",
                Description = "Test Description",
                Category = "Work",
                IsCompleted = false,
                TotalProgress = 50,
                Progressions = Enumerable.Empty<ProgressionView>()
            }
        };

        var output = adapter.FormatItems(items);

        Assert.NotNull(output);
        var enumerable = output as System.Collections.IEnumerable;
        Assert.NotNull(enumerable);
    }
}
