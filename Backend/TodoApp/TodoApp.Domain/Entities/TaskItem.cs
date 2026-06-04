namespace TodoApp.Domain.Entities
{
    public class TaskItem
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool IsCompleted { get; set; } = false;
        public bool IsImportant { get; set; } = false;
        public bool IsMyDay { get; set; } = false;
        public DateTime? DueDate { get; set; }
        public DateTime? ReminderDate { get; set; }
        public RepeatInterval RepeatInterval { get; set; } = RepeatInterval.None;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public int UserId { get; set; }
        public int? TaskListId { get; set; }


        public User User { get; set; } = null!;
        public TaskList? TaskList { get; set; }
        public ICollection<TaskCategory> TaskCategories { get; set; } = new List<TaskCategory>();
    }
}