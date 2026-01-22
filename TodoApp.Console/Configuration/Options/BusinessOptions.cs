namespace TodoApp.Console.Configuration.Options;

public class BusinessOptions
{
    public const string SectionName = "TodoApp:Business";

    public int MaxProgressionPercentage { get; set; } = 100;
    public int AllowUpdateAfterPercentage { get; set; } = 50;
    public List<string> DefaultCategories { get; set; } = new() { "Work", "Personal", "Study", "Health" };
    public int MaxDescriptionLength { get; set; } = 500;
    public int MinDescriptionLength { get; set; } = 3;
}
