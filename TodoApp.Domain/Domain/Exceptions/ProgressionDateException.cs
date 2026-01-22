namespace TodoApp.Domain.Domain.Exceptions;

public class ProgressionDateException : DomainException
{
    public ProgressionDateException(string message) : base(message) { }
}
