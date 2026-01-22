using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using Polly.CircuitBreaker;
using Polly.Retry;
using Polly.Timeout;
using TodoApp.Application.Common.Interfaces;
using TodoApp.Infrastructure.Configuration;

namespace TodoApp.Infrastructure.Resilience;

public class ResiliencePolicyProvider : IResiliencePolicyProvider
{
    private readonly ILogger<ResiliencePolicyProvider> _logger;
    private readonly ResilienceOptions _options;

    public ResiliencePolicyProvider(
        ILogger<ResiliencePolicyProvider> logger,
        IOptions<ResilienceOptions> options)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
    }

    public ResiliencePipeline<T> GetRetryPolicy<T>()
    {
        return new ResiliencePipelineBuilder<T>()
            .AddRetry(new RetryStrategyOptions<T>
            {
                ShouldHandle = new PredicateBuilder<T>()
                    .Handle<Exception>(),
                MaxRetryAttempts = _options.Retry.MaxRetryAttempts,
                Delay = TimeSpan.FromSeconds(_options.Retry.InitialDelaySeconds),
                BackoffType = _options.Retry.UseExponentialBackoff 
                    ? DelayBackoffType.Exponential 
                    : DelayBackoffType.Constant,
                OnRetry = args =>
                {
                    _logger.LogWarning(
                        "Retry attempt {AttemptNumber} after {Delay}ms due to: {Exception}",
                        args.AttemptNumber,
                        args.RetryDelay.TotalMilliseconds,
                        args.Outcome.Exception?.Message ?? "Unknown error");
                    return ValueTask.CompletedTask;
                }
            })
            .Build();
    }

    public ResiliencePipeline<T> GetCircuitBreakerPolicy<T>()
    {
        return new ResiliencePipelineBuilder<T>()
            .AddCircuitBreaker(new CircuitBreakerStrategyOptions<T>
            {
                ShouldHandle = new PredicateBuilder<T>()
                    .Handle<Exception>(),
                FailureRatio = _options.CircuitBreaker.FailureRatio,
                MinimumThroughput = _options.CircuitBreaker.MinimumThroughput,
                SamplingDuration = TimeSpan.FromSeconds(_options.CircuitBreaker.SamplingDurationSeconds),
                BreakDuration = TimeSpan.FromSeconds(_options.CircuitBreaker.BreakDurationSeconds),
                OnOpened = args =>
                {
                    _logger.LogError(
                        "Circuit breaker opened due to: {Exception}",
                        args.Outcome.Exception?.Message ?? "Unknown error");
                    return ValueTask.CompletedTask;
                },
                OnClosed = args =>
                {
                    _logger.LogInformation("Circuit breaker closed, normal operation resumed");
                    return ValueTask.CompletedTask;
                },
                OnHalfOpened = args =>
                {
                    _logger.LogInformation("Circuit breaker half-opened, testing if service recovered");
                    return ValueTask.CompletedTask;
                }
            })
            .Build();
    }

    public ResiliencePipeline<T> GetTimeoutPolicy<T>(TimeSpan timeout)
    {
        return new ResiliencePipelineBuilder<T>()
            .AddTimeout(new TimeoutStrategyOptions
            {
                Timeout = timeout,
                OnTimeout = args =>
                {
                    _logger.LogError(
                        "Operation timed out after {Timeout}ms",
                        args.Timeout.TotalMilliseconds);
                    return ValueTask.CompletedTask;
                }
            })
            .Build();
    }

    public ResiliencePipeline<T> GetDatabasePolicy<T>()
    {
        return CreateTimeoutAndRetryPolicy<T>(
            TimeSpan.FromSeconds(_options.Timeout.DatabaseTimeoutSeconds),
            "Database");
    }

    public ResiliencePipeline<T> GetCommandPolicy<T>()
    {
        return CreateTimeoutAndRetryPolicy<T>(
            TimeSpan.FromSeconds(_options.Timeout.CommandTimeoutSeconds),
            "Command");
    }

    public ResiliencePipeline<T> GetQueryPolicy<T>()
    {
        return CreateTimeoutAndRetryPolicy<T>(
            TimeSpan.FromSeconds(_options.Timeout.QueryTimeoutSeconds),
            "Query");
    }

    private ResiliencePipeline<T> CreateTimeoutAndRetryPolicy<T>(TimeSpan timeout, string operationType)
    {
        return new ResiliencePipelineBuilder<T>()
            .AddTimeout(new TimeoutStrategyOptions
            {
                Timeout = timeout,
                OnTimeout = args =>
                {
                    _logger.LogError(
                        "{OperationType} operation timed out after {Timeout}ms",
                        operationType,
                        args.Timeout.TotalMilliseconds);
                    return ValueTask.CompletedTask;
                }
            })
            .AddRetry(new RetryStrategyOptions<T>
            {
                ShouldHandle = new PredicateBuilder<T>()
                    .Handle<Exception>(),
                MaxRetryAttempts = _options.Retry.MaxRetryAttempts,
                Delay = TimeSpan.FromSeconds(_options.Retry.InitialDelaySeconds),
                BackoffType = _options.Retry.UseExponentialBackoff 
                    ? DelayBackoffType.Exponential 
                    : DelayBackoffType.Constant,
                OnRetry = args =>
                {
                    _logger.LogWarning(
                        "{OperationType} retry attempt {AttemptNumber} after {Delay}ms due to: {Exception}",
                        operationType,
                        args.AttemptNumber,
                        args.RetryDelay.TotalMilliseconds,
                        args.Outcome.Exception?.Message ?? "Unknown error");
                    return ValueTask.CompletedTask;
                }
            })
            .Build();
    }
}
