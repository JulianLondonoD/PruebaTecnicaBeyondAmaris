using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using TodoApp.Infrastructure.Configuration;
using TodoApp.Infrastructure.Resilience;

namespace TodoApp.Tests.Application;

public class ResiliencePolicyProviderTests
{
    private readonly Mock<ILogger<ResiliencePolicyProvider>> _mockLogger;
    private readonly ResilienceOptions _options;
    private readonly ResiliencePolicyProvider _provider;

    public ResiliencePolicyProviderTests()
    {
        _mockLogger = new Mock<ILogger<ResiliencePolicyProvider>>();
        _options = new ResilienceOptions
        {
            Retry = new ResilienceOptions.RetryOptions
            {
                MaxRetryAttempts = 3,
                InitialDelaySeconds = 1,
                UseExponentialBackoff = true
            },
            CircuitBreaker = new ResilienceOptions.CircuitBreakerOptions
            {
                FailureRatio = 0.5,
                MinimumThroughput = 10,
                SamplingDurationSeconds = 30,
                BreakDurationSeconds = 30
            },
            Timeout = new ResilienceOptions.TimeoutOptions
            {
                DefaultTimeoutSeconds = 30,
                DatabaseTimeoutSeconds = 10,
                CommandTimeoutSeconds = 30,
                QueryTimeoutSeconds = 10
            }
        };
        
        _provider = new ResiliencePolicyProvider(_mockLogger.Object, Options.Create(_options));
    }

    [Fact]
    public void GetRetryPolicy_ReturnsPolicy()
    {
        // Act
        var policy = _provider.GetRetryPolicy<string>();

        // Assert
        Assert.NotNull(policy);
    }

    [Fact]
    public void GetCircuitBreakerPolicy_ReturnsPolicy()
    {
        // Act
        var policy = _provider.GetCircuitBreakerPolicy<string>();

        // Assert
        Assert.NotNull(policy);
    }

    [Fact]
    public void GetTimeoutPolicy_ReturnsPolicy()
    {
        // Act
        var policy = _provider.GetTimeoutPolicy<string>(TimeSpan.FromSeconds(5));

        // Assert
        Assert.NotNull(policy);
    }

    [Fact]
    public void GetDatabasePolicy_ReturnsPolicy()
    {
        // Act
        var policy = _provider.GetDatabasePolicy<string>();

        // Assert
        Assert.NotNull(policy);
    }

    [Fact]
    public void GetCommandPolicy_ReturnsPolicy()
    {
        // Act
        var policy = _provider.GetCommandPolicy<string>();

        // Assert
        Assert.NotNull(policy);
    }

    [Fact]
    public void GetQueryPolicy_ReturnsPolicy()
    {
        // Act
        var policy = _provider.GetQueryPolicy<string>();

        // Assert
        Assert.NotNull(policy);
    }

    [Fact]
    public async Task RetryPolicy_RetriesOnFailure()
    {
        // Arrange
        var policy = _provider.GetRetryPolicy<string>();
        var attemptCount = 0;

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(async () =>
        {
            await policy.ExecuteAsync<string>(async _ =>
            {
                attemptCount++;
                await Task.CompletedTask;
                throw new InvalidOperationException("Test exception");
            }, CancellationToken.None);
        });

        // Should retry 3 times + 1 original attempt = 4 total attempts
        Assert.Equal(4, attemptCount);
    }

    [Fact]
    public async Task DatabasePolicy_AppliesTimeoutAndRetry()
    {
        // Arrange
        var policy = _provider.GetDatabasePolicy<string>();
        var result = "success";

        // Act
        var actualResult = await policy.ExecuteAsync<string>(async _ =>
        {
            await Task.CompletedTask;
            return result;
        }, CancellationToken.None);

        // Assert
        Assert.Equal(result, actualResult);
    }
}
