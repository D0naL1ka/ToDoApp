using Microsoft.EntityFrameworkCore;
using TodoApp.Application.DTOs.TaskList;
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
         .Include(tl => tl.Tasks.Where(t => !t.IsCompleted))
         .Where(tl => tl.UserId == userId)
         .OrderBy(tl => tl.CreatedAt)
         .ToListAsync();
        public async Task<IEnumerable<TaskListDto>> GetAllAsync(int userId)
        {
            return await _context.TaskLists
                .Where(list => list.UserId == userId)
                .Select(list => new TaskListDto
                {
                    Id = list.Id,
                    Name = list.Name,
                    TaskCount = list.Tasks.Count(t => !t.IsCompleted)
                })
                .ToListAsync();
        }
    }
}
