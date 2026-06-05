using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TodoApp.Domain.Entities;

namespace TodoApp.Infrastructure.Configuration
{
    public class CategoryConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(x => x.Color)
                .IsRequired()
                .HasMaxLength(7);

            builder.Property(x => x.IsSystem)
                .HasDefaultValue(false);

            builder.Property(x => x.UserId)
                .IsRequired(false);

            builder.HasData(
                new Category { Id = 1, Name = "Червона категорія", Color = "#FF4B4B", IsSystem = true, UserId = null, CreatedAt = new DateTime(2026, 06, 04) },
                new Category { Id = 2, Name = "Оранжева категорія", Color = "#FF8C00", IsSystem = true, UserId = null, CreatedAt = new DateTime(2026, 06, 04) },
                new Category { Id = 3, Name = "Жовта категорія", Color = "#FFD700", IsSystem = true, UserId = null, CreatedAt = new DateTime(2026, 06, 04) },
                new Category { Id = 4, Name = "Зелена категорія", Color = "#4CAF50", IsSystem = true, UserId = null, CreatedAt = new DateTime(2026, 06, 04) },
                new Category { Id = 5, Name = "Синя категорія", Color = "#2196F3", IsSystem = true, UserId = null, CreatedAt = new DateTime(2026, 06, 04) }
            );
        }
    }
}
