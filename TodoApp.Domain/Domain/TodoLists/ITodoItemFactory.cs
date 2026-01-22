using TodoApp.Domain.Domain.Entities;
using TodoApp.Domain.Domain.ValueObjects;

namespace TodoApp.Domain.Domain.TodoLists;

public interface ITodoItemFactory
{
    TodoItem Create(TodoItemId id, string title, string description, string category);
}
