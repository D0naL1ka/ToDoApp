using Microsoft.EntityFrameworkCore;
using TodoApp.Domain.Entities;
using TodoApp.Domain.Interfaces.Repositories;
using TodoApp.Infrastructure.Data;

namespace TodoApp.Infrastructure.Repositories
{
    public class TaskListRepository : BaseRepository<TaskList>, ITaskListRepository
    {
        public TaskListRepository(AppDbContext context) : base(context) { }

        public async Task<IEnumerable<TaskList>> GetByUserIdAsync(int userId)
            => await _context.TaskLists
                .Include(tl => tl.Tasks)
                .Where(tl => tl.UserId == userId)
                .OrderBy(tl => tl.CreatedAt)
                .ToListAsync();
    }
}
