using MediatR;
using Microsoft.Extensions.Logging;
using TodoApp.Application.Application.Commands;
using TodoApp.Domain.Domain.TodoLists;

namespace TodoApp.Application.Application.Handlers;

public class UpdateTodoItemCommandHandler : IRequestHandler<UpdateTodoItemCommand>
{
    private readonly ITodoListRepository _repository;
    private readonly ILogger<UpdateTodoItemCommandHandler> _logger;

    public UpdateTodoItemCommandHandler(ITodoListRepository repository, ILogger<UpdateTodoItemCommandHandler> logger)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task Handle(UpdateTodoItemCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating TodoItem {Id}", request.Id);
        
        var aggregate = await _repository.LoadAsync();
        aggregate.UpdateItem(request.Id, request.Description);
        await _repository.SaveAsync(aggregate);
        
        _logger.LogInformation("Successfully updated TodoItem {Id}", request.Id);
    }
}
