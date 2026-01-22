using MediatR;
using TodoApp.Application.Application.Commands;
using TodoApp.Application.Application.Queries;
using TodoApp.Domain.Domain.ValueObjects;

namespace TodoApp.Application.Application.TodoLists;

public class TodoListApplication : ITodoListApplication
{
    private readonly IMediator _mediator;

    public TodoListApplication(IMediator mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    public async Task AddItemAsync(int id, string title, string description, string category)
    {
        await _mediator.Send(new AddTodoItemCommand(id, title, description, category));
    }

    public async Task UpdateItemAsync(int id, string description)
    {
        await _mediator.Send(new UpdateTodoItemCommand(id, description));
    }

    public async Task RemoveItemAsync(int id)
    {
        await _mediator.Send(new RemoveTodoItemCommand(id));
    }

    public async Task RegisterProgressionAsync(int id, DateTime dateTime, decimal percent)
    {
        await _mediator.Send(new RegisterProgressionCommand(id, dateTime, percent));
    }

    public async Task<IEnumerable<TodoItemView>> GetItemsAsync()
    {
        return await _mediator.Send(new GetTodoItemsQuery());
    }

    public async Task<int> GetNextIdAsync()
    {
        return await _mediator.Send(new GetNextIdQuery());
    }

    public async Task<List<string>> GetCategoriesAsync()
    {
        return await _mediator.Send(new GetCategoriesQuery());
    }
}
