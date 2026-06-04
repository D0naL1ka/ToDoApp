using TodoApp.Application.DTOs.TaskList;

namespace TodoApp.Application.Interfaces.Services
{
    public interface ITaskListService
    {
        Task<IEnumerable<TaskListDto>> GetByUserIdAsync(int userId);
        Task<TaskListDto> GetByIdAsync(int id, int userId);
        Task<TaskListDto> CreateAsync(int userId, CreateTaskListDto dto);
        Task<TaskListDto> UpdateAsync(int id, int userId, UpdateTaskListDto dto);
        Task DeleteAsync(int id, int userId);
    }
}
