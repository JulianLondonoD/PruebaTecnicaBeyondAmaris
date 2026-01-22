using MediatR;

namespace TodoApp.Application.Application.Commands;

public record UpdateTodoItemCommand(int Id, string Description) : IRequest;
