using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TodoApp.Domain.Entities;

namespace TodoApp.Infrastructure.Configuration
{
    public class TaskConfiguration : IEntityTypeConfiguration<TaskItem>
    {
        public void Configure(EntityTypeBuilder<TaskItem> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Title)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(x => x.Description)
                .HasMaxLength(1000);

            builder.Property(x => x.RepeatInterval)
                .HasConversion<string>();

            builder.Property(x => x.TaskListId)
                .IsRequired(false);

            // Task - TaskList
            builder.HasOne(x => x.TaskList)
                .WithMany(x => x.Tasks)
                .HasForeignKey(x => x.TaskListId)
                .OnDelete(DeleteBehavior.NoAction);

            // Task - User
            builder.HasOne(x => x.User)
                .WithMany(x => x.Tasks)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
