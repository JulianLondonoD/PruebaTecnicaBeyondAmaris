using MediatR;
using Microsoft.Extensions.Logging;
using Polly;
using TodoApp.Application.Common.Interfaces;

namespace TodoApp.Application.Behaviors;

public class ResilienceBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ILogger<ResilienceBehavior<TRequest, TResponse>> _logger;
    private readonly IResiliencePolicyProvider _resiliencePolicyProvider;

    public ResilienceBehavior(
        ILogger<ResilienceBehavior<TRequest, TResponse>> logger,
        IResiliencePolicyProvider resiliencePolicyProvider)
    {
        _logger = logger;
        _resiliencePolicyProvider = resiliencePolicyProvider;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        
        // Determine if this is a Command or Query based on naming convention
        var isCommand = requestName.Contains("Command", StringComparison.OrdinalIgnoreCase);
        var isQuery = requestName.Contains("Query", StringComparison.OrdinalIgnoreCase);

        ResiliencePipeline<TResponse>? policy = null;

        if (isCommand)
        {
            _logger.LogDebug("Applying command resilience policy to {RequestName}", requestName);
            policy = _resiliencePolicyProvider.GetCommandPolicy<TResponse>();
        }
        else if (isQuery)
        {
            _logger.LogDebug("Applying query resilience policy to {RequestName}", requestName);
            policy = _resiliencePolicyProvider.GetQueryPolicy<TResponse>();
        }

        if (policy != null)
        {
            return await policy.ExecuteAsync(async ct =>
            {
                return await next();
            }, cancellationToken);
        }
        else
        {
            // No resilience policy for this request type
            return await next();
        }
    }
}
