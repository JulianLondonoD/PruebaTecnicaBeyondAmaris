using MediatR;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using TodoApp.Application.Application.Queries;

namespace TodoApp.Api.HealthChecks;

public class BusinessRulesHealthCheck : IHealthCheck
{
    private readonly IMediator _mediator;

    public BusinessRulesHealthCheck(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            var categories = await _mediator.Send(new GetCategoriesQuery(), cancellationToken);
            var isHealthy = categories.Count >= 4;
            
            var data = new Dictionary<string, object>
            {
                ["categories_count"] = categories.Count,
                ["categories"] = string.Join(", ", categories)
            };

            return isHealthy 
                ? HealthCheckResult.Healthy("Business rules OK", data) 
                : HealthCheckResult.Degraded("Missing categories", null, data);
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("Business rules failed", ex);
        }
    }
}
