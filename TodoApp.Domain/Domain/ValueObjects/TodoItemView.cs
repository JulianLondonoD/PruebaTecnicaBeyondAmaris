namespace TodoApp.Domain.Domain.ValueObjects;

public record TodoItemView
{
    public int Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string Category { get; init; } = string.Empty;
    public bool IsCompleted { get; init; }
    public decimal TotalProgress { get; init; }
    public IEnumerable<ProgressionView> Progressions { get; init; } = [];
}

public record ProgressionView
{
    public DateTime DateTime { get; init; }
    public decimal Percent { get; init; }
    public decimal AccumulatedPercent { get; init; }
}
