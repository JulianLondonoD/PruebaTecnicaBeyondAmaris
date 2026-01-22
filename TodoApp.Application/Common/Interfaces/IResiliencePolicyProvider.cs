using Polly;

namespace TodoApp.Application.Common.Interfaces;

public interface IResiliencePolicyProvider
{
    ResiliencePipeline<T> GetRetryPolicy<T>();
    ResiliencePipeline<T> GetCircuitBreakerPolicy<T>();
    ResiliencePipeline<T> GetTimeoutPolicy<T>(TimeSpan timeout);
    ResiliencePipeline<T> GetDatabasePolicy<T>();
    ResiliencePipeline<T> GetCommandPolicy<T>();
    ResiliencePipeline<T> GetQueryPolicy<T>();
}
