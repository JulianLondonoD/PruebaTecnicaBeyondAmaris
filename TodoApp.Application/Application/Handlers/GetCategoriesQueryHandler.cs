using MediatR;
using TodoApp.Application.Application.Queries;
using TodoApp.Domain.Domain.TodoLists;

namespace TodoApp.Application.Application.Handlers;

public class GetCategoriesQueryHandler : IRequestHandler<GetCategoriesQuery, List<string>>
{
    private readonly ITodoListRepository _repository;

    public GetCategoriesQueryHandler(ITodoListRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    public async Task<List<string>> Handle(GetCategoriesQuery request, CancellationToken cancellationToken)
    {
        return await _repository.GetCategoriesAsync();
    }
}
