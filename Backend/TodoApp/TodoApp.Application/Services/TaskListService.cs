using AutoMapper;
using TodoApp.Application.DTOs.TaskList;
using TodoApp.Application.Interfaces.Services;
using TodoApp.Domain.Entities;
using TodoApp.Domain.Interfaces.Repositories;

namespace TodoApp.Application.Services
{
    public class TaskListService : ITaskListService
    {
        private readonly ITaskListRepository _taskListRepository;
        private readonly ITaskRepository _taskRepository;
        private readonly IMapper _mapper;

        public TaskListService(ITaskListRepository taskListRepository, ITaskRepository taskRepository, IMapper mapper)
        {
            _taskListRepository = taskListRepository;
            _taskRepository = taskRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<TaskListDto>> GetByUserIdAsync(int userId)
        {
            var lists = await _taskListRepository.GetByUserIdAsync(userId);
            return _mapper.Map<IEnumerable<TaskListDto>>(lists);
        }

        public async Task<TaskListDto> GetByIdAsync(int id, int userId)
        {
            var list = await _taskListRepository.GetByIdAsync(id);
            if (list == null || list.UserId != userId)
                throw new Exception("List not found");
            return _mapper.Map<TaskListDto>(list);
        }

        public async Task<TaskListDto> CreateAsync(int userId, CreateTaskListDto dto)
        {
            var list = new TaskList { Name = dto.Name, UserId = userId };
            var created = await _taskListRepository.CreateAsync(list);
            return _mapper.Map<TaskListDto>(created);
        }

        public async Task<TaskListDto> UpdateAsync(int id, int userId, UpdateTaskListDto dto)
        {
            var list = await _taskListRepository.GetByIdAsync(id);
            if (list == null || list.UserId != userId)
                throw new Exception("List not found");
            list.Name = dto.Name;
            var updated = await _taskListRepository.UpdateAsync(list);
            return _mapper.Map<TaskListDto>(updated);
        }

        public async Task DeleteAsync(int id, int userId)
        {
            var list = await _taskListRepository.GetByIdAsync(id);
            if (list == null || list.UserId != userId)
                throw new Exception("List not found");

            var tasks = await _taskRepository.GetByTaskListAsync(id, userId);
            foreach (var task in tasks)
            {
                await _taskRepository.DeleteAsync(task.Id);
            }

            await _taskListRepository.DeleteAsync(id);
        }

    }
}
