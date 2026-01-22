using MediatR;

namespace TodoApp.Application.Application.Commands;

public record RemoveTodoItemCommand(int Id) : IRequest;
