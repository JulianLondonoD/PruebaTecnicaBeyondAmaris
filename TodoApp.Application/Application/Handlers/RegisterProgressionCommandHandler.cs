using MediatR;
using Microsoft.Extensions.Logging;
using TodoApp.Application.Application.Commands;
using TodoApp.Domain.Domain.TodoLists;

namespace TodoApp.Application.Application.Handlers;

public class RegisterProgressionCommandHandler : IRequestHandler<RegisterProgressionCommand>
{
    private readonly ITodoListRepository _repository;
    private readonly ILogger<RegisterProgressionCommandHandler> _logger;

    public RegisterProgressionCommandHandler(ITodoListRepository repository, ILogger<RegisterProgressionCommandHandler> logger)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task Handle(RegisterProgressionCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Registering progression for TodoItem {Id}: {Percent}%", request.Id, request.Percent);
        
        var aggregate = await _repository.LoadAsync();
        aggregate.RegisterProgression(request.Id, request.DateTime, request.Percent);
        await _repository.SaveAsync(aggregate);
        
        _logger.LogInformation("Successfully registered progression for TodoItem {Id}", request.Id);
    }
}
