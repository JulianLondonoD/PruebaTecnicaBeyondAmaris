using TodoApp.Domain.Domain.Aggregates;

namespace TodoApp.Domain.Domain.TodoLists;

public interface ITodoListRepository
{
    Task<TodoListAggregate> LoadAsync();
    Task SaveAsync(TodoListAggregate aggregate);
    Task<int> GetNextIdAsync();
    Task<List<string>> GetCategoriesAsync();
}
