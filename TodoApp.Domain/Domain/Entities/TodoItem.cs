using TodoApp.Domain.Domain.Exceptions;
using TodoApp.Domain.Domain.ValueObjects;

namespace TodoApp.Domain.Domain.Entities;

public class TodoItem
{
    public TodoItemId Id { get; private set; }
    public string Title { get; private set; }
    public string Description { get; private set; }
    public string Category { get; private set; }
    public List<Progression> Progressions { get; private set; }

    public bool IsCompleted => Progressions.Sum(p => p.Percent) >= 100;
    
    public decimal CurrentProgress => Progressions.Sum(p => p.Percent);

    public TodoItem(TodoItemId id, string title, string description, string category)
    {
        Id = id;
        Title = title ?? throw new ArgumentNullException(nameof(title));
        Description = description ?? throw new ArgumentNullException(nameof(description));
        Category = category ?? throw new ArgumentNullException(nameof(category));
        Progressions = new List<Progression>();
    }

    public void Update(string description)
    {
        if (CurrentProgress > 50)
            throw new TodoItemProgressException($"Cannot update TodoItem with ID {Id} because progress is greater than 50%");
        
        Description = description ?? throw new ArgumentNullException(nameof(description));
    }

    public void AddProgression(DateTime dateTime, decimal percent)
    {
        if (Progressions.Any() && dateTime <= Progressions.Max(p => p.DateTime))
            throw new ProgressionDateException($"Progression date must be greater than existing progressions");

        if (percent <= 0 || percent > 100)
            throw new ProgressionPercentException($"Percent must be greater than 0 and less than or equal to 100");

        var newTotal = CurrentProgress + percent;
        if (newTotal > 100)
            throw new ProgressionPercentException($"Sum of all percents cannot exceed 100%. Current: {CurrentProgress}%, Trying to add: {percent}%");

        Progressions.Add(new Progression(dateTime, percent));
    }
}
