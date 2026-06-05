using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TodoApp.Application.DTOs.Common;
using TodoApp.Application.DTOs.Task;
using TodoApp.Application.Interfaces.Services;

namespace TodoApp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TasksController : ControllerBase
    {
        private readonly ITaskService _taskService;
        private int UserId => int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        public TasksController(ITaskService taskService)
        {
            _taskService = taskService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] TaskFilterDto filter)
        {
            var result = await _taskService.GetPagedAsync(UserId, filter);
            return Ok(result);
        }

        [HttpGet("myday")]
        public async Task<IActionResult> GetMyDay()
        {
            var result = await _taskService.GetMyDayAsync(UserId);
            return Ok(result);
        }

        [HttpGet("important")]
        public async Task<IActionResult> GetImportant()
        {
            var result = await _taskService.GetImportantAsync(UserId);
            return Ok(result);
        }

        [HttpGet("planned")]
        public async Task<IActionResult> GetPlanned()
        {
            var result = await _taskService.GetPlannedAsync(UserId);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _taskService.GetByIdAsync(id, UserId);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateTaskDto dto)
        {
            var result = await _taskService.CreateAsync(UserId, dto);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateTaskDto dto)
        {
            var result = await _taskService.UpdateAsync(id, UserId, dto);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _taskService.DeleteAsync(id, UserId);
            return NoContent();
        }
    }
}
