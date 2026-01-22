using TodoApp.Domain.Domain.ValueObjects;

namespace TodoApp.Domain.Domain.TodoLists;

public interface ITodoList
{
    void AddItem(int id, string title, string description, string category);
    void UpdateItem(int id, string description);
    void RemoveItem(int id);
    void RegisterProgression(int id, DateTime dateTime, decimal percent);
    IEnumerable<TodoItemView> GetAllItems();
}
