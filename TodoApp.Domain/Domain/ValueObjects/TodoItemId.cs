namespace TodoApp.Domain.Domain.ValueObjects;

public record TodoItemId
{
    public int Value { get; init; }

    public TodoItemId(int value)
    {
        if (value <= 0)
            throw new ArgumentException("TodoItem ID must be greater than 0", nameof(value));
        
        Value = value;
    }

    public static implicit operator int(TodoItemId id) => id.Value;
    public static implicit operator TodoItemId(int value) => new(value);
}
