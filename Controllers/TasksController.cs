using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using AuthDemo.Api.DTOs;
using AuthDemo.Api.Services;

namespace AuthDemo.Api.Controllers
{
    [Authorize]
    [Route("api/tasks")]
    [ApiController]
    public class TasksController : ControllerBase
    {
        private readonly ITaskService _taskService;

        public TasksController(ITaskService taskService)
        {
            _taskService = taskService;
        }

        private Guid? GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Guid.TryParse(userIdClaim, out var userId) ? userId : null;
        }

        // GET /api/tasks
        [HttpGet]
        public async Task<IActionResult> GetTasks([FromQuery] string? status, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var userId = GetCurrentUserId();
            if (userId == null) return Unauthorized();

            var (tasks, totalItems) = await _taskService.GetTasksAsync(userId.Value, status, page, pageSize);

            return Ok(new
            {
                success = true,
                message = "Lấy danh sách task thành công",
                data = new { totalItems, page, pageSize, items = tasks }
            });
        }

        // GET /api/tasks/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTaskById(int id)
        {
            var userId = GetCurrentUserId();
            if (userId == null) return Unauthorized();

            var task = await _taskService.GetTaskByIdAsync(id, userId.Value);

            if (task == null)
                return NotFound(new { success = false, message = "Không tìm thấy task hoặc bạn không có quyền xem." });

            return Ok(new { success = true, message = "Lấy chi tiết thành công", data = task });
        }

        // POST /api/tasks
        [HttpPost]
        public async Task<IActionResult> CreateTask([FromBody] CreateTaskDto dto)
        {
            var userId = GetCurrentUserId();
            if (userId == null) return Unauthorized();

            var task = await _taskService.CreateTaskAsync(userId.Value, dto);

            return CreatedAtAction(nameof(GetTaskById), new { id = task.Id }, new { success = true, message = "Tạo task thành công", data = task });
        }

        // PUT /api/tasks/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTask(int id, [FromBody] UpdateTaskDto dto)
        {
            var userId = GetCurrentUserId();
            if (userId == null) return Unauthorized();

            var task = await _taskService.UpdateTaskAsync(id, userId.Value, dto);
            if (task == null)
                return NotFound(new { success = false, message = "Không tìm thấy task hoặc không có quyền sửa." });

            return Ok(new { success = true, message = "Cập nhật task thành công", data = task });
        }

        // DELETE /api/tasks/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTask(int id)
        {
            var userId = GetCurrentUserId();
            if (userId == null) return Unauthorized();

            var success = await _taskService.DeleteTaskAsync(id, userId.Value);
            if (!success)
                return NotFound(new { success = false, message = "Không tìm thấy task hoặc không có quyền xóa." });

            return Ok(new { success = true, message = "Xóa task thành công" });
        }
    }
}