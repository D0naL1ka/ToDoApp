namespace TodoApp.Application.DTOs.TaskList
{
    public class TaskListDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int TaskCount { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
