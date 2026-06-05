using TodoApp.Domain.Entities;

namespace TodoApp.Domain.Interfaces.Repositories
{
    public interface ITaskListRepository : IBaseRepository<TaskList>
    {
        Task<IEnumerable<TaskList>> GetByUserIdAsync(int userId);
    }
}
