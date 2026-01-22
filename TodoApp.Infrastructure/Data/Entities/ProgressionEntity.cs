using System.ComponentModel.DataAnnotations;

namespace TodoApp.Infrastructure.Data.Entities;

public class ProgressionEntity
{
    public int Id { get; set; }
    
    public int TodoItemId { get; set; }
    
    public DateTime DateTime { get; set; }
    
    [Range(0.01, 100)]
    public decimal Percent { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public virtual TodoItemEntity TodoItem { get; set; } = null!;
}
