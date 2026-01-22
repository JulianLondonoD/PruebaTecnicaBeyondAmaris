using FluentAssertions;
using TodoApp.Domain.Domain.ValueObjects;
using TodoApp.Tests.Application.Tests.Common;
using TodoApp.Tests.Application.Tests.Common.Builders;

namespace TodoApp.Tests.Application.Tests.Common.Examples;

public class BuilderExampleTests
{
    [Fact]
    public void TodoItemBuilder_ShouldCreateTodoItemWithDefaults()
    {
        var todoItem = new TodoItemBuilder().Build();

        todoItem.Should().NotBeNull();
        todoItem.Title.Should().Be("Default Todo Title");
        todoItem.Category.Should().Be("Work");
    }

    [Fact]
    public void TodoItemBuilder_ShouldCreateTodoItemWithCustomValues()
    {
        var todoItem = new TodoItemBuilder()
            .WithId(42)
            .WithTitle("My Custom Title")
            .WithDescription("My custom description")
            .WithCategory("Personal")
            .Build();

        todoItem.Id.Value.Should().Be(42);
        todoItem.Title.Should().Be("My Custom Title");
        todoItem.Description.Should().Be("My custom description");
        todoItem.Category.Should().Be("Personal");
    }

    [Fact]
    public void TodoItemBuilder_ShouldCreateCompletedTodoItem()
    {
        var todoItem = new TodoItemBuilder()
            .WithTitle("Completed Task")
            .Completed()
            .Build();

        todoItem.IsCompleted.Should().BeTrue();
        todoItem.CurrentProgress.Should().Be(100);
    }

    [Fact]
    public void TodoItemBuilder_ShouldCreateInProgressTodoItem()
    {
        var todoItem = new TodoItemBuilder()
            .WithTitle("In Progress Task")
            .InProgress(50)
            .Build();

        todoItem.IsCompleted.Should().BeFalse();
        todoItem.CurrentProgress.Should().Be(50);
    }

    [Fact]
    public void TodoItemBuilder_ShouldAddMultipleProgressions()
    {
        var baseDate = DateTime.Now;
        var todoItem = new TodoItemBuilder()
            .WithProgression(baseDate, 25)
            .WithProgression(baseDate.AddDays(1), 25)
            .WithProgression(baseDate.AddDays(2), 25)
            .Build();

        todoItem.CurrentProgress.Should().Be(75);
        todoItem.Progressions.Should().HaveCount(3);
    }

    [Fact]
    public void CategoryBuilder_ShouldCreateWorkCategory()
    {
        var category = new CategoryBuilder().Work().Build();

        category.Should().Be("Work");
    }

    [Fact]
    public void CategoryBuilder_ShouldCreateCustomCategory()
    {
        var category = new CategoryBuilder().Custom("MyCategory").Build();

        category.Should().Be("MyCategory");
    }

    [Fact]
    public void TestDataGenerator_ShouldGenerateRandomTodoItem()
    {
        var todoItem = TestDataGenerator.GenerateRandomTodoItem();

        todoItem.Should().NotBeNull();
        todoItem.Title.Should().NotBeNullOrEmpty();
        todoItem.Description.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void TestDataGenerator_ShouldGenerateTodoItemWithProgress()
    {
        var todoItem = TestDataGenerator.GenerateTodoItemWithProgress(3);

        todoItem.Should().NotBeNull();
        todoItem.Progressions.Should().NotBeEmpty();
        todoItem.Progressions.Should().HaveCountGreaterThan(0);
    }

    [Fact]
    public void TestDataGenerator_ShouldGenerateCompletedTodoItem()
    {
        var todoItem = TestDataGenerator.GenerateCompletedTodoItem();

        todoItem.Should().NotBeNull();
        todoItem.IsCompleted.Should().BeTrue();
    }

    [Fact]
    public void TestDataGenerator_ShouldGenerateProgression()
    {
        var progression = TestDataGenerator.GenerateRandomProgression();

        progression.Should().NotBeNull();
        progression.Percent.Should().BeGreaterThan(0);
        progression.Percent.Should().BeLessThanOrEqualTo(100);
    }
}
