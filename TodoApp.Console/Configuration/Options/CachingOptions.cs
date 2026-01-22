namespace TodoApp.Console.Configuration.Options;

public class CachingOptions
{
    public const string SectionName = "TodoApp:Caching";

    public int DefaultExpirationMinutes { get; set; } = 60;
    public string CategoriesCacheKey { get; set; } = "categories";
    public string ConfigurationCacheKey { get; set; } = "configuration";
    public bool Enabled { get; set; } = true;
}
