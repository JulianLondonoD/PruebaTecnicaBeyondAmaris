using TodoApp.Domain.Domain.ValueObjects;
using System.Text.Json;

namespace TodoApp.Infrastructure.Adapters.Output;

public class JsonOutputAdapter : ITodoItemOutputAdapter<string>
{
    public string FormatItems(IEnumerable<TodoItemView> items)
    {
        return JsonSerializer.Serialize(items, new JsonSerializerOptions 
        { 
            WriteIndented = true 
        });
    }
}
