using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TodoApp.Application.DTOs.TaskList;
using TodoApp.Application.Interfaces.Services;

namespace TodoApp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TaskListsController : ControllerBase
    {
        private readonly ITaskListService _taskListService;
        private readonly ITaskService _taskService;
        private int UserId => int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        public TaskListsController(ITaskListService taskListService, ITaskService taskService)
        {
            _taskListService = taskListService;
            _taskService = taskService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _taskListService.GetByUserIdAsync(UserId);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _taskListService.GetByIdAsync(id, UserId);
            return Ok(result);
        }

        [HttpGet("{id}/tasks")]
        public async Task<IActionResult> GetTasks(int id)
        {
            var result = await _taskService.GetByTaskListAsync(id, UserId);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateTaskListDto dto)
        {
            var result = await _taskListService.CreateAsync(UserId, dto);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateTaskListDto dto)
        {
            var result = await _taskListService.UpdateAsync(id, UserId, dto);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _taskListService.DeleteAsync(id, UserId);
            return NoContent();
        }
    }
}
