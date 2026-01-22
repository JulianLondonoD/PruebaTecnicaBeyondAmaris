namespace TodoApp.Console.Configuration.Options;

public class DatabaseOptions
{
    public const string SectionName = "TodoApp:Database";

    public string ConnectionString { get; set; } = "Data Source=todoapp.db";
    public int CommandTimeout { get; set; } = 30;
    public bool EnableSensitiveDataLogging { get; set; } = false;
    public bool EnableDetailedErrors { get; set; } = false;
}
