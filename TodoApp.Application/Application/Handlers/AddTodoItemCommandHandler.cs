using MediatR;
using Microsoft.Extensions.Logging;
using TodoApp.Application.Application.Commands;
using TodoApp.Domain.Domain.TodoLists;

namespace TodoApp.Application.Application.Handlers;

public class AddTodoItemCommandHandler : IRequestHandler<AddTodoItemCommand>
{
    private readonly ITodoListRepository _repository;
    private readonly ILogger<AddTodoItemCommandHandler> _logger;

    public AddTodoItemCommandHandler(ITodoListRepository repository, ILogger<AddTodoItemCommandHandler> logger)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task Handle(AddTodoItemCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Adding TodoItem {Id}: {Title}", request.Id, request.Title);
        
        // Load existing aggregate from database
        var aggregate = await _repository.LoadAsync();
        
        // Add new item to aggregate
        aggregate.AddItem(request.Id, request.Title, request.Description, request.Category);
        
        // Save aggregate back to database
        await _repository.SaveAsync(aggregate);
        
        _logger.LogInformation("Successfully added TodoItem {Id}", request.Id);
    }
}
