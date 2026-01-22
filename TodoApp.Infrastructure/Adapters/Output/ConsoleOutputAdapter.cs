using TodoApp.Domain.Domain.ValueObjects;
using System.Text;

namespace TodoApp.Infrastructure.Adapters.Output;

public class ConsoleOutputAdapter : ITodoItemOutputAdapter<string>
{
    public string FormatItems(IEnumerable<TodoItemView> items)
    {
        var sb = new StringBuilder();
        
        foreach (var item in items)
        {
            sb.AppendLine($"{item.Id}) {item.Title} - {item.Description} ({item.Category}) Completed:{item.IsCompleted}");
            
            foreach (var progression in item.Progressions)
            {
                var progressBar = CreateProgressBar(progression.AccumulatedPercent);
                sb.AppendLine($"{progression.DateTime:M/d/yyyy h:mm:ss tt} - {progression.AccumulatedPercent}%\t{progressBar}");
            }
            
            if (item.Progressions.Any())
                sb.AppendLine();
        }
        
        return sb.ToString();
    }
    
    private string CreateProgressBar(decimal progress)
    {
        const int totalBars = 50;
        var filledBars = (int)Math.Round((progress / 100m) * totalBars);
        var emptyBars = totalBars - filledBars;
        
        return "|" + new string('O', filledBars) + new string(' ', emptyBars) + "|";
    }
}
