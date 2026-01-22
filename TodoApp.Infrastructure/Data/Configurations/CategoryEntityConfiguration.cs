using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TodoApp.Infrastructure.Data.Entities;

namespace TodoApp.Infrastructure.Data.Configurations;

public class CategoryEntityConfiguration : IEntityTypeConfiguration<CategoryEntity>
{
    public void Configure(EntityTypeBuilder<CategoryEntity> builder)
    {
        builder.ToTable("Categories");
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Name).IsRequired().HasMaxLength(100);
        
        builder.HasData(
            new CategoryEntity { Id = 1, Name = "Work" },
            new CategoryEntity { Id = 2, Name = "Personal" },
            new CategoryEntity { Id = 3, Name = "Study" },
            new CategoryEntity { Id = 4, Name = "Health" }
        );
    }
}
