using TodoApp.Domain.Domain.Entities;
using TodoApp.Domain.Domain.TodoLists;
using TodoApp.Domain.Domain.ValueObjects;

namespace TodoApp.Domain.Domain.Factories;

public class TodoItemFactory : ITodoItemFactory
{
    public TodoItem Create(TodoItemId id, string title, string description, string category)
    {
        return new TodoItem(id, title, description, category);
    }
}
