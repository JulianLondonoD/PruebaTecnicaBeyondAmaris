namespace TodoApp.Domain.Domain.Exceptions;

public class TodoItemProgressException : DomainException
{
    public TodoItemProgressException(string message) : base(message) { }
}
