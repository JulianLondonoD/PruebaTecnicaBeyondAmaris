namespace TodoApp.Console.Configuration.Options;

public class ApplicationOptions
{
    public const string SectionName = "TodoApp:Application";

    public string Name { get; set; } = "TodoApp Enterprise";
    public string Version { get; set; } = "2.0.0";
    public string Environment { get; set; } = "Development";
}
