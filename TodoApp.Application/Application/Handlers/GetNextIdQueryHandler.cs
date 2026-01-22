using MediatR;
using TodoApp.Application.Application.Queries;
using TodoApp.Domain.Domain.TodoLists;

namespace TodoApp.Application.Application.Handlers;

public class GetNextIdQueryHandler : IRequestHandler<GetNextIdQuery, int>
{
    private readonly ITodoListRepository _repository;

    public GetNextIdQueryHandler(ITodoListRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    public async Task<int> Handle(GetNextIdQuery request, CancellationToken cancellationToken)
    {
        return await _repository.GetNextIdAsync();
    }
}
