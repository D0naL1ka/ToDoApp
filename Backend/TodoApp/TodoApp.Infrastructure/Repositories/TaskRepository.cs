using Microsoft.EntityFrameworkCore;
using TodoApp.Domain.Entities;
using TodoApp.Domain.Interfaces.Repositories;
using TodoApp.Infrastructure.Data;

namespace TodoApp.Infrastructure.Repositories
{
    public class TaskRepository : BaseRepository<TaskItem>, ITaskRepository
    {
        public TaskRepository(AppDbContext context) : base(context) { }

        public async Task<(IEnumerable<TaskItem> Items, int TotalCount)> GetPagedAsync(
            int userId, int page, int pageSize,
            string? search, int? categoryId, bool? grouped)
        {
            var query = _context.Tasks
                .Include(t => t.TaskList)
                .Include(t => t.TaskCategories)
                    .ThenInclude(tc => tc.Category)
                .Where(t => t.UserId == userId);

            if (!string.IsNullOrEmpty(search))
                query = query.Where(t => t.Title.Contains(search));

            if (categoryId.HasValue)
                query = query.Where(t =>
                    t.TaskCategories.Any(tc => tc.CategoryId == categoryId));

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderByDescending(t => t.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }

        public async Task<IEnumerable<TaskItem>> GetMyDayAsync(int userId)
            => await _context.Tasks
                .Include(t => t.TaskCategories).ThenInclude(tc => tc.Category)
                .Where(t => t.UserId == userId && t.IsMyDay)
                .ToListAsync();

        public async Task<IEnumerable<TaskItem>> GetImportantAsync(int userId)
            => await _context.Tasks
                .Include(t => t.TaskCategories).ThenInclude(tc => tc.Category)
                .Where(t => t.UserId == userId && t.IsImportant)
                .ToListAsync();

        public async Task<IEnumerable<TaskItem>> GetPlannedAsync(int userId)
            => await _context.Tasks
                .Include(t => t.TaskCategories).ThenInclude(tc => tc.Category)
                .Where(t => t.UserId == userId && t.DueDate.HasValue)
                .OrderBy(t => t.DueDate)
                .ToListAsync();

        public async Task<IEnumerable<TaskItem>> GetByTaskListAsync(int taskListId, int userId)
            => await _context.Tasks
                .Include(t => t.TaskCategories).ThenInclude(tc => tc.Category)
                .Where(t => t.TaskListId == taskListId && t.UserId == userId)
                .ToListAsync();
    }
}
