using MediatR;
using Microsoft.Extensions.Logging;
using TodoApp.Application.Application.Queries;
using TodoApp.Domain.Domain.TodoLists;
using TodoApp.Domain.Domain.ValueObjects;

namespace TodoApp.Application.Application.Handlers;

public class GetTodoItemsQueryHandler : IRequestHandler<GetTodoItemsQuery, IEnumerable<TodoItemView>>
{
    private readonly ITodoListRepository _repository;
    private readonly ILogger<GetTodoItemsQueryHandler> _logger;

    public GetTodoItemsQueryHandler(ITodoListRepository repository, ILogger<GetTodoItemsQueryHandler> logger)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<IEnumerable<TodoItemView>> Handle(GetTodoItemsQuery request, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Getting all TodoItems");
        
        var aggregate = await _repository.LoadAsync();
        var items = aggregate.GetAllItems();
        
        _logger.LogDebug("Retrieved {Count} TodoItems", items.Count());
        return items;
    }
}
