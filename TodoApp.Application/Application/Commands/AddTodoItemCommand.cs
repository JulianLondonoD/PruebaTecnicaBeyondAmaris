using MediatR;

namespace TodoApp.Application.Application.Commands;

public record AddTodoItemCommand(int Id, string Title, string Description, string Category) : IRequest;
