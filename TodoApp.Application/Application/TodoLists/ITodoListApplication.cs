using TodoApp.Domain.Domain.ValueObjects;

namespace TodoApp.Application.Application.TodoLists;

public interface ITodoListApplication
{
    Task AddItemAsync(int id, string title, string description, string category);
    Task UpdateItemAsync(int id, string description);
    Task RemoveItemAsync(int id);
    Task RegisterProgressionAsync(int id, DateTime dateTime, decimal percent);
    Task<IEnumerable<TodoItemView>> GetItemsAsync();
    Task<int> GetNextIdAsync();
    Task<List<string>> GetCategoriesAsync();
}
