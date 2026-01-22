using MediatR;
using Microsoft.Extensions.Logging;
using TodoApp.Application.Common.Exceptions;

namespace TodoApp.Application.Behaviors;

public class ExceptionHandlingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ILogger<ExceptionHandlingBehavior<TRequest, TResponse>> _logger;

    public ExceptionHandlingBehavior(ILogger<ExceptionHandlingBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        try
        {
            return await next();
        }
        catch (ValidationException ex)
        {
            _logger.LogWarning(
                ex,
                "Error de validación en {RequestName}: {Errors}",
                typeof(TRequest).Name,
                ex.Errors);
            
            throw;
        }
        catch (BusinessRuleException ex)
        {
            _logger.LogWarning(
                ex,
                "Violación de regla de negocio en {RequestName}: {RuleName} - {Message}",
                typeof(TRequest).Name,
                ex.RuleName,
                ex.Message);
            
            throw;
        }
        catch (InfrastructureException ex)
        {
            _logger.LogError(
                ex,
                "Error de infraestructura en {RequestName}: {Component} - {Operation}",
                typeof(TRequest).Name,
                ex.Component,
                ex.OperationName ?? "N/A");
            
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error no controlado en {RequestName}: {Message}",
                typeof(TRequest).Name,
                ex.Message);
            
            throw;
        }
    }
}
