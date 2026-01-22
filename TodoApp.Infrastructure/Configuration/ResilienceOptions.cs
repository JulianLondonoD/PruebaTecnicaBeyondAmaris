namespace TodoApp.Infrastructure.Configuration;

public class ResilienceOptions
{
    public const string SectionName = "TodoApp:Resilience";
    
    public RetryOptions Retry { get; set; } = new();
    public CircuitBreakerOptions CircuitBreaker { get; set; } = new();
    public TimeoutOptions Timeout { get; set; } = new();
    
    public class RetryOptions
    {
        public int MaxRetryAttempts { get; set; } = 3;
        public int InitialDelaySeconds { get; set; } = 1;
        public bool UseExponentialBackoff { get; set; } = true;
    }
    
    public class CircuitBreakerOptions
    {
        public double FailureRatio { get; set; } = 0.5;
        public int MinimumThroughput { get; set; } = 10;
        public int SamplingDurationSeconds { get; set; } = 30;
        public int BreakDurationSeconds { get; set; } = 30;
    }
    
    public class TimeoutOptions
    {
        public int DefaultTimeoutSeconds { get; set; } = 30;
        public int DatabaseTimeoutSeconds { get; set; } = 10;
        public int CommandTimeoutSeconds { get; set; } = 30;
        public int QueryTimeoutSeconds { get; set; } = 10;
    }
}
