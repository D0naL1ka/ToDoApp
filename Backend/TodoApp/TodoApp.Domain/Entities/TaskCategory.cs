namespace TodoApp.Domain.Entities
{
    public class TaskCategory
    {
        public int TaskId { get; set; }
        public int CategoryId { get; set; }


        public TaskItem Task { get; set; } = null!;
        public Category Category { get; set; } = null!;
    }
}
