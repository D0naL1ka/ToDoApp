using AutoMapper;
using TodoApp.Application.DTOs.Common;
using TodoApp.Application.DTOs.Task;
using TodoApp.Application.Interfaces.Services;
using TodoApp.Domain.Entities;
using TodoApp.Domain.Interfaces.Repositories;

namespace TodoApp.Application.Services
{
    public class TaskService : ITaskService
    {
        private readonly ITaskRepository _taskRepository;
        private readonly IMapper _mapper;

        public TaskService(ITaskRepository taskRepository, IMapper mapper)
        {
            _taskRepository = taskRepository;
            _mapper = mapper;
        }

        public async Task<PagedResultDto<TaskDto>> GetPagedAsync(int userId, TaskFilterDto filter)
        {
            var (items, totalCount) = await _taskRepository.GetPagedAsync(
                userId, filter.Page, filter.PageSize,
                filter.Search, filter.CategoryId, filter.Grouped);

            return new PagedResultDto<TaskDto>
            {
                Items = _mapper.Map<IEnumerable<TaskDto>>(items),
                TotalCount = totalCount,
                Page = filter.Page,
                PageSize = filter.PageSize
            };
        }

        public async Task<IEnumerable<TaskDto>> GetMyDayAsync(int userId)
        {
            var tasks = await _taskRepository.GetMyDayAsync(userId);
            return _mapper.Map<IEnumerable<TaskDto>>(tasks);
        }

        public async Task<IEnumerable<TaskDto>> GetImportantAsync(int userId)
        {
            var tasks = await _taskRepository.GetImportantAsync(userId);
            return _mapper.Map<IEnumerable<TaskDto>>(tasks);
        }

        public async Task<IEnumerable<TaskDto>> GetPlannedAsync(int userId)
        {
            var tasks = await _taskRepository.GetPlannedAsync(userId);
            return _mapper.Map<IEnumerable<TaskDto>>(tasks);
        }

        public async Task<IEnumerable<TaskDto>> GetByTaskListAsync(int taskListId, int userId)
        {
            var tasks = await _taskRepository.GetByTaskListAsync(taskListId, userId);
            return _mapper.Map<IEnumerable<TaskDto>>(tasks);
        }

        public async Task<TaskDto> GetByIdAsync(int id, int userId)
        {
            var task = await _taskRepository.GetByIdAsync(id);
            if (task == null || task.UserId != userId)
                throw new Exception("Task not found");
            return _mapper.Map<TaskDto>(task);
        }

        public async Task<TaskDto> CreateAsync(int userId, CreateTaskDto dto)
        {
            var task = new TaskItem
            {
                Title = dto.Title,
                TaskListId = dto.TaskListId,
                UserId = userId,
                TaskCategories = dto.CategoryIds.Select(categoryId => new TaskCategory
                {
                    CategoryId = categoryId
                }).ToList()
            };

            var created = await _taskRepository.CreateAsync(task);
            return _mapper.Map<TaskDto>(created);
        }

        public async Task<TaskDto> UpdateAsync(int id, int userId, UpdateTaskDto dto)
        {
            // в Infrastructure.
            var task = await _taskRepository.GetByIdAsync(id);
            if (task == null || task.UserId != userId)
                throw new Exception("Task not found");

            task.Title = dto.Title;
            task.Description = dto.Description;
            task.IsCompleted = dto.IsCompleted;
            task.IsImportant = dto.IsImportant;
            task.IsMyDay = dto.IsMyDay;
            task.DueDate = dto.DueDate;
            task.ReminderDate = dto.ReminderDate;
            task.RepeatInterval = dto.RepeatInterval;
            task.TaskListId = dto.TaskListId;

            task.TaskCategories.Clear();
            foreach (var categoryId in dto.CategoryIds)
            {
                task.TaskCategories.Add(new TaskCategory
                {
                    TaskId = id,
                    CategoryId = categoryId
                });
            }

            var updated = await _taskRepository.UpdateAsync(task);
            return _mapper.Map<TaskDto>(updated);
        }

        public async Task DeleteAsync(int id, int userId)
        {
            var task = await _taskRepository.GetByIdAsync(id);
            if (task == null || task.UserId != userId)
                throw new Exception("Task not found");
            await _taskRepository.DeleteAsync(id);
        }
    }
}