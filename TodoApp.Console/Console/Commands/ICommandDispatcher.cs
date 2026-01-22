namespace TodoApp.Console.Console.Commands;

public interface ICommandDispatcher
{
    Task<bool> DispatchAsync(string input);
    IEnumerable<ICommandHandler> GetAllHandlers();
}
