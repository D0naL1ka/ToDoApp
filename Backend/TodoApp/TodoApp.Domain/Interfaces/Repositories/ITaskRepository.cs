using TodoApp.Domain.Entities;

namespace TodoApp.Domain.Interfaces.Repositories;

public interface ITaskRepository : IBaseRepository<TaskItem>
{
    Task<(IEnumerable<TaskItem> Items, int TotalCount)> GetPagedAsync(
        int userId,
        int page,
        int pageSize,
        string? search,
        int? categoryId,
        bool? grouped,
        bool? isCompleted = null, 
        bool? isUnassigned = null);

    Task<IEnumerable<TaskItem>> GetMyDayAsync(int userId);
    Task<IEnumerable<TaskItem>> GetImportantAsync(int userId);
    Task<IEnumerable<TaskItem>> GetPlannedAsync(int userId);
    Task<IEnumerable<TaskItem>> GetByTaskListAsync(int taskListId, int userId);
    Task<TaskItem?> GetByIdNoTrackingAsync(int id);
    Task<TaskItem> UpdateWithCategoriesAsync(TaskItem task, List<int> categoryIds);
}