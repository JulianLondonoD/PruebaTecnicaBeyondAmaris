using Microsoft.EntityFrameworkCore;
using TodoApp.Infrastructure.Data.Configurations;
using TodoApp.Infrastructure.Data.Entities;

namespace TodoApp.Infrastructure.Data;

public class TodoDbContext : DbContext
{
    public DbSet<CategoryEntity> Categories { get; set; }
    public DbSet<TodoItemEntity> TodoItems { get; set; }
    public DbSet<ProgressionEntity> Progressions { get; set; }

    public TodoDbContext(DbContextOptions<TodoDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfiguration(new CategoryEntityConfiguration());

        // TodoItems configuration
        modelBuilder.Entity<TodoItemEntity>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedNever(); // Manual ID assignment
            entity.Property(e => e.Title).HasMaxLength(200).IsRequired();
            entity.Property(e => e.Description).HasMaxLength(1000).IsRequired();
            entity.Property(e => e.Category).HasMaxLength(100).IsRequired();
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.UpdatedAt);
            
            entity.ToTable("TodoItems");
            
            // Indexes for better performance
            entity.HasIndex(e => e.Category);
            entity.HasIndex(e => e.CreatedAt);
        });

        // Progressions configuration
        modelBuilder.Entity<ProgressionEntity>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.DateTime).IsRequired();
            entity.Property(e => e.Percent).HasColumnType("decimal(5,2)").IsRequired();
            entity.Property(e => e.CreatedAt).IsRequired();
            
            entity.ToTable("Progressions");
            
            // Foreign key relationship
            entity.HasOne(e => e.TodoItem)
                  .WithMany(e => e.Progressions)
                  .HasForeignKey(e => e.TodoItemId)
                  .OnDelete(DeleteBehavior.Cascade);
                  
            // Indexes for better performance
            entity.HasIndex(e => e.TodoItemId);
            entity.HasIndex(e => e.DateTime);
        });
    }
}
