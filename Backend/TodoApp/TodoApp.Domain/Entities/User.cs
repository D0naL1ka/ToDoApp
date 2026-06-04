namespace TodoApp.Domain.Entities;

public class User
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;


    public ICollection<TaskItem> Tasks { get; set; } = new List<TaskItem>();
    public ICollection<TaskList> TaskLists { get; set; } = new List<TaskList>();
    public ICollection<Category> Categories { get; set; } = new List<Category>();
    public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
}