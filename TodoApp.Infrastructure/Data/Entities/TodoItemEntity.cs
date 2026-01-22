using System.ComponentModel.DataAnnotations;

namespace TodoApp.Infrastructure.Data.Entities;

public class TodoItemEntity
{
    public int Id { get; set; }
    
    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(1000)]
    public string Description { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(100)]
    public string Category { get; set; } = string.Empty;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    
    public virtual ICollection<ProgressionEntity> Progressions { get; set; } = new List<ProgressionEntity>();
    
    // Computed properties
    public bool IsCompleted => Progressions.Sum(p => p.Percent) >= 100;
    public decimal TotalProgress => Progressions.Sum(p => p.Percent);
}
