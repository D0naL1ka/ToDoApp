namespace TodoApp.Application.DTOs.Task
{
    public class CreateTaskDto
    {
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsImportant { get; set; }
        public bool IsMyDay { get; set; }
        public DateTime? DueDate { get; set; }

        public int? TaskListId { get; set; }

        public List<int> CategoryIds { get; set; } = new();
    }
}
