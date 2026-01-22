using Microsoft.Extensions.Diagnostics.HealthChecks;
using TodoApp.Domain.Domain.TodoLists;

namespace TodoApp.Infrastructure.HealthChecks;

public class BusinessRulesHealthCheck : IHealthCheck
{
    private readonly ITodoListDomainService _domainService;
    private readonly ITodoListRepository _repository;

    public BusinessRulesHealthCheck(
        ITodoListDomainService domainService,
        ITodoListRepository repository)
    {
        _domainService = domainService ?? throw new ArgumentNullException(nameof(domainService));
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var categories = await _repository.GetCategoriesAsync();
            var nextId = await _repository.GetNextIdAsync();

            var data = new Dictionary<string, object>
            {
                { "domainServiceConfigured", _domainService != null },
                { "repositoryConfigured", _repository != null },
                { "categoryCount", categories.Count },
                { "nextAvailableId", nextId }
            };

            return HealthCheckResult.Healthy(
                "Business rules and domain services are properly configured",
                data);
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy(
                "Business rules configuration failed",
                ex,
                new Dictionary<string, object>
                {
                    { "error", ex.Message }
                });
        }
    }
}
