using TodoApp.Domain.Domain.Entities;
using TodoApp.Domain.Domain.Exceptions;
using TodoApp.Domain.Domain.TodoLists;
using TodoApp.Domain.Domain.ValueObjects;

namespace TodoApp.Domain.Domain.Aggregates;

public class TodoListAggregate : ITodoList
{
    private readonly Dictionary<int, TodoItem> _items = new();
    private readonly ITodoItemFactory _factory;
    private readonly ITodoListDomainService _domainService;

    public IReadOnlyList<TodoItem> Items => _items.Values.OrderBy(x => x.Id.Value).ToList();

    public TodoListAggregate(ITodoItemFactory factory, ITodoListDomainService domainService)
    {
        _factory = factory ?? throw new ArgumentNullException(nameof(factory));
        _domainService = domainService ?? throw new ArgumentNullException(nameof(domainService));
    }

    public void LoadExistingItem(TodoItem item)
    {
        if (item == null)
            throw new ArgumentNullException(nameof(item));
            
        _items[item.Id.Value] = item;
    }

    public void AddItem(int id, string title, string description, string category)
    {
        if (!_domainService.ValidateCategory(category))
            throw new DomainException($"Invalid category: {category}");

        var todoItem = _factory.Create(id, title, description, category);
        _items[id] = todoItem;
    }

    public void UpdateItem(int id, string description)
    {
        if (!_items.TryGetValue(id, out var item))
            throw new DomainException($"TodoItem with ID {id} not found");

        item.Update(description);
    }

    public void RemoveItem(int id)
    {
        if (!_items.TryGetValue(id, out var item))
            throw new DomainException($"TodoItem with ID {id} not found");

        if (item.CurrentProgress > 50)
            throw new TodoItemProgressException($"Cannot remove TodoItem with ID {id} because progress is greater than 50%");

        _items.Remove(id);
    }

    public void RegisterProgression(int id, DateTime dateTime, decimal percent)
    {
        if (!_items.TryGetValue(id, out var item))
            throw new DomainException($"TodoItem with ID {id} not found");

        item.AddProgression(dateTime, percent);
    }

    public IEnumerable<TodoItemView> GetAllItems()
    {
        return _items.Values
            .OrderBy(item => item.Id.Value)
            .Select(item => new TodoItemView
            {
                Id = item.Id.Value,
                Title = item.Title,
                Description = item.Description,
                Category = item.Category,
                IsCompleted = item.IsCompleted,
                TotalProgress = item.CurrentProgress,
                Progressions = item.Progressions
                    .OrderBy(p => p.DateTime)
                    .Select((progression, index) => new ProgressionView
                    {
                        DateTime = progression.DateTime,
                        Percent = progression.Percent,
                        AccumulatedPercent = item.Progressions
                            .OrderBy(p => p.DateTime)
                            .Take(index + 1)
                            .Sum(p => p.Percent)
                    })
            });
    }
}
