namespace TodoApp.Console.Console.Commands;

public interface ICommandHandler
{
    string CommandName { get; }
    string Description { get; }
    Task<bool> CanHandle(string command);
    Task<bool> HandleAsync(string[] args);
}
