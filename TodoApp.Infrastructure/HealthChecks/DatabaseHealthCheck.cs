using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using TodoApp.Infrastructure.Data;

namespace TodoApp.Infrastructure.HealthChecks;

public class DatabaseHealthCheck : IHealthCheck
{
    private readonly TodoDbContext _context;

    public DatabaseHealthCheck(TodoDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await _context.Database.CanConnectAsync(cancellationToken);

            var categoryCount = await _context.Categories.CountAsync(cancellationToken);

            var data = new Dictionary<string, object>
            {
                { "database", "connected" },
                { "categoryCount", categoryCount },
                { "providerName", _context.Database.ProviderName ?? "unknown" }
            };

            return HealthCheckResult.Healthy(
                "Database is healthy and queryable",
                data);
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy(
                "Database connection failed",
                ex,
                new Dictionary<string, object>
                {
                    { "database", "disconnected" },
                    { "error", ex.Message }
                });
        }
    }
}
