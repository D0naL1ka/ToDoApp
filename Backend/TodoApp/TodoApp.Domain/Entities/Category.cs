namespace TodoApp.Domain.Entities
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Color { get; set; } = "#000000";
        public bool IsSystem { get; set; } = false;
        public int? UserId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;


        public User? User { get; set; }
        public ICollection<TaskCategory> TaskCategories { get; set; } = new List<TaskCategory>();
    }
}
