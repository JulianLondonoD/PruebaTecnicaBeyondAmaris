namespace TodoApp.Application.Common.Exceptions;

public class InfrastructureException : Exception
{
    public string Component { get; }
    public string? OperationName { get; }

    public InfrastructureException(string message)
        : base(message)
    {
        Component = "Unknown";
    }

    public InfrastructureException(string component, string message)
        : base(message)
    {
        Component = component;
    }

    public InfrastructureException(string component, string operationName, string message)
        : base(message)
    {
        Component = component;
        OperationName = operationName;
    }

    public InfrastructureException(string component, string message, Exception innerException)
        : base(message, innerException)
    {
        Component = component;
    }
}
