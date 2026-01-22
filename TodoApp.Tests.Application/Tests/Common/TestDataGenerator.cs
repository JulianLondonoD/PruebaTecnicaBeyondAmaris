using AutoFixture;
using TodoApp.Domain.Domain.Entities;
using TodoApp.Domain.Domain.ValueObjects;
using TodoApp.Tests.Application.Tests.Common.Builders;

namespace TodoApp.Tests.Application.Tests.Common;

public static class TestDataGenerator
{
    private const int MinProgressPercent = 1;
    private const int MaxDaysInPast = 30;
    
    private static readonly Fixture _fixture = new();
    private static readonly Random _random = new();

    public static TodoItem GenerateRandomTodoItem()
    {
        var id = _random.Next(1, 10000);
        var titleText = _fixture.Create<string>();
        var title = titleText.Substring(0, Math.Min(50, titleText.Length));
        var description = _fixture.Create<string>();
        var category = GetRandomCategory();

        return new TodoItemBuilder()
            .WithId(id)
            .WithTitle(title)
            .WithDescription(description)
            .WithCategory(category)
            .Build();
    }

    public static TodoItem GenerateTodoItemWithProgress(int progressCount)
    {
        if (progressCount <= 0)
            throw new ArgumentException("Progress count must be greater than 0", nameof(progressCount));

        var builder = new TodoItemBuilder()
            .WithId(_random.Next(1, 10000))
            .WithTitle(_fixture.Create<string>())
            .WithDescription(_fixture.Create<string>())
            .WithCategory(GetRandomCategory());

        var totalPercent = 0m;
        var maxPercentPerProgression = 100m / progressCount;
        var baseDate = DateTime.Now.AddDays(-progressCount);

        for (int i = 0; i < progressCount; i++)
        {
            var remainingPercent = 100m - totalPercent;
            var maxPossible = Math.Min(maxPercentPerProgression, remainingPercent);
            var percent = Math.Max(MinProgressPercent, (decimal)_random.NextDouble() * maxPossible);
            
            if (i == progressCount - 1 && totalPercent + percent < 100)
            {
                percent = Math.Min(remainingPercent, maxPercentPerProgression);
            }

            totalPercent += percent;
            builder.WithProgression(baseDate.AddDays(i), percent);

            if (totalPercent >= 100)
                break;
        }

        return builder.Build();
    }

    public static Progression GenerateRandomProgression()
    {
        var dateTime = DateTime.Now.AddDays(-_random.Next(0, MaxDaysInPast));
        var percent = (decimal)(_random.NextDouble() * 99 + MinProgressPercent);
        return new Progression(dateTime, percent);
    }

    public static Progression GenerateProgression(DateTime dateTime, decimal percent)
    {
        return new Progression(dateTime, percent);
    }

    public static TodoItem GenerateCompletedTodoItem()
    {
        return new TodoItemBuilder()
            .WithId(_random.Next(1, 10000))
            .WithTitle(_fixture.Create<string>())
            .WithDescription(_fixture.Create<string>())
            .WithCategory(GetRandomCategory())
            .Completed()
            .Build();
    }

    public static TodoItem GenerateInProgressTodoItem(decimal percent = 50)
    {
        return new TodoItemBuilder()
            .WithId(_random.Next(1, 10000))
            .WithTitle(_fixture.Create<string>())
            .WithDescription(_fixture.Create<string>())
            .WithCategory(GetRandomCategory())
            .InProgress(percent)
            .Build();
    }

    public static TodoItem GenerateNotStartedTodoItem()
    {
        return new TodoItemBuilder()
            .WithId(_random.Next(1, 10000))
            .WithTitle(_fixture.Create<string>())
            .WithDescription(_fixture.Create<string>())
            .WithCategory(GetRandomCategory())
            .Build();
    }

    public static IEnumerable<TodoItem> GenerateMultipleTodoItems(int count)
    {
        for (int i = 0; i < count; i++)
        {
            yield return GenerateRandomTodoItem();
        }
    }

    public static string GetRandomCategory()
    {
        var categories = new[]
        {
            CategoryBuilder.CommonCategories.Work,
            CategoryBuilder.CommonCategories.Personal,
            CategoryBuilder.CommonCategories.Study,
            CategoryBuilder.CommonCategories.Health,
            CategoryBuilder.CommonCategories.Finance,
            CategoryBuilder.CommonCategories.Shopping,
            CategoryBuilder.CommonCategories.Home
        };

        return categories[_random.Next(categories.Length)];
    }

    public static TodoItemId GenerateRandomId()
    {
        return new TodoItemId(_random.Next(1, 10000));
    }
}
