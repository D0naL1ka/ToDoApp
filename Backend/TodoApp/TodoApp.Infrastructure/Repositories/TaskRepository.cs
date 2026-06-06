using Microsoft.EntityFrameworkCore;
using TodoApp.Domain.Entities;
using TodoApp.Domain.Interfaces.Repositories;
using TodoApp.Infrastructure.Data;

namespace TodoApp.Infrastructure.Repositories;

public class TaskRepository : BaseRepository<TaskItem>, ITaskRepository
{
    public TaskRepository(AppDbContext context) : base(context) { }

    public async Task<(IEnumerable<TaskItem> Items, int TotalCount)> GetPagedAsync(
        int userId, int page, int pageSize,
        string? search, int? categoryId, bool? grouped,
        bool? isCompleted = null, bool? isUnassigned = null)
    {
        var query = _context.Tasks
                .Include(t => t.TaskList)
                .Include(t => t.TaskCategories).ThenInclude(tc => tc.Category)
                .Where(t => t.UserId == userId);

        if (!string.IsNullOrEmpty(search))
            query = query.Where(t => t.Title.Contains(search));

        if (categoryId.HasValue)
            query = query.Where(t =>
                t.TaskCategories.Any(tc => tc.CategoryId == categoryId));

        if (isCompleted.HasValue)
            query = query.Where(t => t.IsCompleted == isCompleted.Value);

        if (isUnassigned == true)
            query = query.Where(t => t.TaskListId == null);

        var totalCount = await query.CountAsync();

        var items = await query.OrderByDescending(t => t.CreatedAt).Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
        return (items, totalCount);
    }

    public override async Task<TaskItem?> GetByIdAsync(int id)
    {
        return await _context.Tasks
            .Include(t => t.TaskCategories)
                .ThenInclude(tc => tc.Category)
            .FirstOrDefaultAsync(t => t.Id == id);
    }

    public async Task<TaskItem?> GetByIdNoTrackingAsync(int id)
    {
        return await _context.Tasks
            .AsNoTracking()
            .Include(t => t.TaskCategories)
                .ThenInclude(tc => tc.Category)
            .FirstOrDefaultAsync(t => t.Id == id);
    }

    public async Task<TaskItem> UpdateWithCategoriesAsync(
     TaskItem task, List<int> categoryIds)
    {
        _context.Tasks.Update(task);

        var existing = await _context.Set<TaskCategory>()
            .Where(tc => tc.TaskId == task.Id)
            .ToListAsync();

        var toRemove = existing
            .Where(tc => !categoryIds.Contains(tc.CategoryId))
            .ToList();
        _context.Set<TaskCategory>().RemoveRange(toRemove);

        var existingIds = existing.Select(tc => tc.CategoryId).ToList();
        var toAdd = categoryIds
            .Where(id => !existingIds.Contains(id))
            .Select(id => new TaskCategory { TaskId = task.Id, CategoryId = id })
            .ToList();
        await _context.Set<TaskCategory>().AddRangeAsync(toAdd);
        await _context.SaveChangesAsync();
        return task;
    }

    public async Task<IEnumerable<TaskItem>> GetMyDayAsync(int userId)
             => await _context.Tasks.Include(t => t.TaskCategories).ThenInclude(tc => tc.Category)
                 .Where(t => t.UserId == userId && t.IsMyDay && !t.IsCompleted).ToListAsync();

    public async Task<IEnumerable<TaskItem>> GetImportantAsync(int userId)
            => await _context.Tasks.Include(t => t.TaskCategories).ThenInclude(tc => tc.Category)
                .Where(t => t.UserId == userId && t.IsImportant && !t.IsCompleted).ToListAsync();

    public async Task<IEnumerable<TaskItem>> GetPlannedAsync(int userId)
        => await _context.Tasks.Include(t => t.TaskCategories).ThenInclude(tc => tc.Category)
            .Where(t => t.UserId == userId && t.DueDate.HasValue && !t.IsCompleted).OrderBy(t => t.DueDate).ToListAsync();

    public async Task<IEnumerable<TaskItem>> GetByTaskListAsync(int taskListId, int userId)
    {
        return await _context.Tasks
            .Include(t => t.TaskCategories).ThenInclude(tc => tc.Category)
            .Where(t => t.UserId == userId && t.TaskListId == taskListId && !t.IsCompleted)
            .ToListAsync();
    }
}