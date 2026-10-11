using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using AuthDemo.Api.Data;
using AuthDemo.Api.DTOs;
using AuthDemo.Api.Models;

namespace AuthDemo.Api.Controllers
{
    [Authorize]
    [Route("api/tasks")]
    [ApiController]
    public class TasksController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TasksController(AppDbContext context)
        {
            _context = context;
        }

        // GET /api/tasks
        [HttpGet]
        public async Task<IActionResult> GetTasks([FromQuery] string? status, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null) return Unauthorized();
            Guid userId = Guid.Parse(userIdClaim);

            var query = _context.Tasks.Where(t => t.UserId == userId);

            if (!string.IsNullOrEmpty(status))
            {
                query = query.Where(t => t.Status == status);
            }

            var totalItems = await query.CountAsync();
            var tasks = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(t => new TaskResponseDto
                {
                    Id = t.Id,
                    Title = t.Title,
                    Description = t.Description,
                    Deadline = t.Deadline,
                    Status = t.Status,
                    CategoryId = t.CategoryId ?? 0
                })
                .ToListAsync();

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
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null) return Unauthorized();
            Guid userId = Guid.Parse(userIdClaim);

            var task = await _context.Tasks
                .Where(t => t.Id == id && t.UserId == userId)
                .Select(t => new TaskResponseDto
                {
                    Id = t.Id,
                    Title = t.Title,
                    Description = t.Description,
                    Deadline = t.Deadline,
                    Status = t.Status,
                    CategoryId = t.CategoryId ?? 0
                })
                .FirstOrDefaultAsync();

            if (task == null)
                return NotFound(new { success = false, message = "Không tìm thấy task hoặc bạn không có quyền xem." });

            return Ok(new { success = true, message = "Lấy chi tiết thành công", data = task });
        }

        // POST /api/tasks
        [HttpPost]
        public async Task<IActionResult> CreateTask([FromBody] CreateTaskDto dto)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null) return Unauthorized();
            Guid userId = Guid.Parse(userIdClaim);

            var task = new TaskItem
            {
                Title = dto.Title,
                Description = dto.Description,
                Deadline = dto.Deadline,
                Status = "Pending",
                UserId = userId,
                CategoryId = dto.CategoryId
            };

            _context.Tasks.Add(task);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTaskById), new { id = task.Id }, new { success = true, message = "Tạo task thành công", data = task });
        }

        // PUT /api/tasks/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTask(int id, [FromBody] UpdateTaskDto dto)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null) return Unauthorized();
            Guid userId = Guid.Parse(userIdClaim);

            var task = await _context.Tasks.FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);
            if (task == null)
                return NotFound(new { success = false, message = "Không tìm thấy task hoặc không có quyền sửa." });

            task.Title = dto.Title;
            task.Description = dto.Description;
            task.Deadline = dto.Deadline;
            task.Status = dto.Status;
            task.CategoryId = dto.CategoryId;

            await _context.SaveChangesAsync();

            return Ok(new { success = true, message = "Cập nhật task thành công", data = task });
        }

        // DELETE /api/tasks/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTask(int id)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null) return Unauthorized();
            Guid userId = Guid.Parse(userIdClaim);

            var task = await _context.Tasks.FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);
            if (task == null)
                return NotFound(new { success = false, message = "Không tìm thấy task hoặc không có quyền xóa." });

            _context.Tasks.Remove(task);
            await _context.SaveChangesAsync();

            return Ok(new { success = true, message = "Xóa task thành công" });
        }
    }
}