using TodoApp.Application.DTOs.Category;
using TodoApp.Domain.Entities;

namespace TodoApp.Application.DTOs.Task
{
    public class TaskDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsCompleted { get; set; }
        public bool IsImportant { get; set; }
        public bool IsMyDay { get; set; }
        public DateTime? DueDate { get; set; }
        public DateTime? ReminderDate { get; set; }
        public RepeatInterval RepeatInterval { get; set; }
        public DateTime CreatedAt { get; set; }
        public int? TaskListId { get; set; }
        public string? TaskListName { get; set; }
        public List<CategoryDto> Categories { get; set; } = new();
    }
}
