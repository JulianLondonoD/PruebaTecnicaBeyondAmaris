namespace TodoApp.Console.Configuration.Options;

public class LoggingOptions
{
    public const string SectionName = "TodoApp:Logging";

    public bool EnablePerformanceLogging { get; set; } = true;
    public int PerformanceThresholdMs { get; set; } = 1000;
    public bool EnableStructuredLogging { get; set; } = true;
}
