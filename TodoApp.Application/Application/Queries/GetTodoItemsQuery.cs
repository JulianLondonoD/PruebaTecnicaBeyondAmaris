using MediatR;
using TodoApp.Domain.Domain.ValueObjects;

namespace TodoApp.Application.Application.Queries;

public record GetTodoItemsQuery() : IRequest<IEnumerable<TodoItemView>>;
