using MediatR;

namespace TodoApp.Application.Application.Commands;

public record RegisterProgressionCommand(int Id, DateTime DateTime, decimal Percent) : IRequest;
