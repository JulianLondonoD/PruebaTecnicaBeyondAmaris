using TodoApp.Domain.Domain.ValueObjects;

namespace TodoApp.Infrastructure.Adapters.Output;

public class WebOutputAdapter : ITodoItemOutputAdapter<IEnumerable<object>>
{
    public IEnumerable<object> FormatItems(IEnumerable<TodoItemView> items)
    {
        return items.Select(item => new
        {
            id = item.Id,
            title = item.Title,
            description = item.Description,
            category = item.Category,
            isCompleted = item.IsCompleted,
            progress = item.TotalProgress,
            progressions = item.Progressions.Select(p => new
            {
                dateTime = p.DateTime,
                percent = p.Percent,
                accumulatedPercent = p.AccumulatedPercent
            })
        });
    }
}
