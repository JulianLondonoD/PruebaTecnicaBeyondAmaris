using MediatR;
using Microsoft.Extensions.Logging;
using TodoApp.Application.Application.Commands;
using TodoApp.Domain.Domain.TodoLists;

namespace TodoApp.Application.Application.Handlers;

public class RemoveTodoItemCommandHandler : IRequestHandler<RemoveTodoItemCommand>
{
    private readonly ITodoListRepository _repository;
    private readonly ILogger<RemoveTodoItemCommandHandler> _logger;

    public RemoveTodoItemCommandHandler(ITodoListRepository repository, ILogger<RemoveTodoItemCommandHandler> logger)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task Handle(RemoveTodoItemCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Removing TodoItem {Id}", request.Id);
        
        var aggregate = await _repository.LoadAsync();
        aggregate.RemoveItem(request.Id);
        await _repository.SaveAsync(aggregate);
        
        _logger.LogInformation("Successfully removed TodoItem {Id}", request.Id);
    }
}
