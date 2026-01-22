using TodoApp.Domain.Domain.Entities;
using TodoApp.Domain.Domain.Factories;
using TodoApp.Domain.Domain.ValueObjects;

namespace TodoApp.Tests.Application.Tests.Common.Builders;

public class TodoItemBuilder
{
    private TodoItemId _id = new(1);
    private string _title = "Default Todo Title";
    private string _description = "Default todo description";
    private string _category = "Work";
    private readonly List<(DateTime dateTime, decimal percent)> _progressions = new();
    private readonly TodoItemFactory _factory = new();

    public TodoItemBuilder WithId(int id)
    {
        _id = new TodoItemId(id);
        return this;
    }

    public TodoItemBuilder WithId(TodoItemId id)
    {
        _id = id;
        return this;
    }

    public TodoItemBuilder WithTitle(string title)
    {
        _title = title;
        return this;
    }

    public TodoItemBuilder WithDescription(string description)
    {
        _description = description;
        return this;
    }

    public TodoItemBuilder WithCategory(string category)
    {
        _category = category;
        return this;
    }

    public TodoItemBuilder WithProgression(DateTime dateTime, decimal percent)
    {
        _progressions.Add((dateTime, percent));
        return this;
    }

    public TodoItemBuilder WithProgressions(params (DateTime dateTime, decimal percent)[] progressions)
    {
        _progressions.AddRange(progressions);
        return this;
    }

    public TodoItemBuilder WithProgress(decimal percent)
    {
        _progressions.Add((DateTime.Now, percent));
        return this;
    }

    public TodoItemBuilder Completed()
    {
        _progressions.Clear();
        _progressions.Add((DateTime.Now, 100));
        return this;
    }

    public TodoItemBuilder InProgress(decimal percent = 50)
    {
        _progressions.Clear();
        _progressions.Add((DateTime.Now, percent));
        return this;
    }

    public TodoItem Build()
    {
        var todoItem = _factory.Create(_id, _title, _description, _category);
        
        var sortedProgressions = _progressions.OrderBy(p => p.dateTime).ToList();
        foreach (var (dateTime, percent) in sortedProgressions)
        {
            todoItem.AddProgression(dateTime, percent);
        }

        return todoItem;
    }

    public static TodoItemBuilder Default() => new();
}
