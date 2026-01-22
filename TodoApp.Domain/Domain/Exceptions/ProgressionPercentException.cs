namespace TodoApp.Domain.Domain.Exceptions;

public class ProgressionPercentException : DomainException
{
    public ProgressionPercentException(string message) : base(message) { }
}
