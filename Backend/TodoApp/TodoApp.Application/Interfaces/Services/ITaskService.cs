using TodoApp.Application.DTOs.Common;
using TodoApp.Application.DTOs.Task;

namespace TodoApp.Application.Interfaces.Services
{
    public interface ITaskService
    {
        Task<PagedResultDto<TaskDto>> GetPagedAsync(int userId, TaskFilterDto filter);
        Task<IEnumerable<TaskDto>> GetMyDayAsync(int userId);
        Task<IEnumerable<TaskDto>> GetImportantAsync(int userId);
        Task<IEnumerable<TaskDto>> GetPlannedAsync(int userId);
        Task<IEnumerable<TaskDto>> GetByTaskListAsync(int taskListId, int userId);
        Task<TaskDto> GetByIdAsync(int id, int userId);
        Task<TaskDto> CreateAsync(int userId, CreateTaskDto dto);
        Task<TaskDto> UpdateAsync(int id, int userId, UpdateTaskDto dto);
        Task DeleteAsync(int id, int userId);
    }
}
