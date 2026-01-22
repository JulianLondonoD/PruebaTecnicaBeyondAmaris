using MediatR;

namespace TodoApp.Application.Application.Queries;

public record GetCategoriesQuery() : IRequest<List<string>>;
