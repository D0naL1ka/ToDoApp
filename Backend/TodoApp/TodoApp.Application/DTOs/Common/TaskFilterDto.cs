namespace TodoApp.Application.DTOs.Common
{
    public class TaskFilterDto
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? Search { get; set; }
        public int? CategoryId { get; set; }
        public bool Grouped { get; set; } = false;

        public bool? IsCompleted { get; set; }
        public bool? IsUnassigned { get; set; }
    }
}
