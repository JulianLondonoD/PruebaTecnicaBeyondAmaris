using MediatR;

namespace TodoApp.Application.Application.Queries;

public record GetNextIdQuery() : IRequest<int>;
