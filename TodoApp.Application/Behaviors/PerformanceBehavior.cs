using MediatR;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace TodoApp.Application.Behaviors;

public class PerformanceBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ILogger<PerformanceBehavior<TRequest, TResponse>> _logger;
    private readonly Stopwatch _timer;

    public PerformanceBehavior(ILogger<PerformanceBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
        _timer = new Stopwatch();
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        _timer.Start();

        var requestName = typeof(TRequest).Name;

        _logger.LogInformation(
            "Iniciando request {RequestName} {@Request}",
            requestName,
            request);

        try
        {
            var response = await next();

            _timer.Stop();

            var elapsedMilliseconds = _timer.ElapsedMilliseconds;

            if (elapsedMilliseconds > 500)
            {
                _logger.LogWarning(
                    "Request {RequestName} de larga duración detectado ({ElapsedMilliseconds}ms) {@Request}",
                    requestName,
                    elapsedMilliseconds,
                    request);
            }
            else
            {
                _logger.LogInformation(
                    "Request {RequestName} completado en {ElapsedMilliseconds}ms",
                    requestName,
                    elapsedMilliseconds);
            }

            return response;
        }
        catch (Exception ex)
        {
            _timer.Stop();
            
            _logger.LogError(
                ex,
                "Request {RequestName} falló después de {ElapsedMilliseconds}ms {@Request}",
                requestName,
                _timer.ElapsedMilliseconds,
                request);
            
            throw;
        }
    }
}
