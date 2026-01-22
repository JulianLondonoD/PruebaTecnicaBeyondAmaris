using TodoApp.Domain.Domain.ValueObjects;

namespace TodoApp.Infrastructure.Adapters.Output;

public interface ITodoItemOutputAdapter<T>
{
    T FormatItems(IEnumerable<TodoItemView> items);
}
