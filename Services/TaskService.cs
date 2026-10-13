using Microsoft.EntityFrameworkCore;
using AuthDemo.Api.Data;
using AuthDemo.Api.DTOs;
using AuthDemo.Api.Models;

namespace AuthDemo.Api.Services
{
    public interface ITaskService
    {
        Task<(IEnumerable<TaskResponseDto> Items, int TotalItems)> GetTasksAsync(Guid userId, string? status, int page, int pageSize);
        Task<TaskResponseDto?> GetTaskByIdAsync(int id, Guid userId);
        Task<TaskItem> CreateTaskAsync(Guid userId, CreateTaskDto dto);
        Task<TaskItem?> UpdateTaskAsync(int id, Guid userId, UpdateTaskDto dto);
        Task<bool> DeleteTaskAsync(int id, Guid userId);
    }

    public class TaskService : ITaskService
    {
        private readonly AppDbContext _context;

        public TaskService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<(IEnumerable<TaskResponseDto> Items, int TotalItems)> GetTasksAsync(Guid userId, string? status, int page, int pageSize)
        {
            var query = _context.Tasks.Where(t => t.UserId == userId);

            if (!string.IsNullOrEmpty(status))
            {
                query = query.Where(t => t.Status == status);
            }

            var totalItems = await query.CountAsync();

            var tasks = await query
                .OrderByDescending(t => t.Deadline) // Thêm OrderBy để phân trang chuẩn, hết cảnh báo EF Core
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

            return (tasks, totalItems);
        }

        public async Task<TaskResponseDto?> GetTaskByIdAsync(int id, Guid userId)
        {
            return await _context.Tasks
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
        }

        public async Task<TaskItem> CreateTaskAsync(Guid userId, CreateTaskDto dto)
        {
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

            return task;
        }

        public async Task<TaskItem?> UpdateTaskAsync(int id, Guid userId, UpdateTaskDto dto)
        {
            var task = await _context.Tasks.FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);
            if (task == null) return null;

            task.Title = dto.Title;
            task.Description = dto.Description;
            task.Deadline = dto.Deadline;
            task.Status = dto.Status;
            task.CategoryId = dto.CategoryId;

            await _context.SaveChangesAsync();
            return task;
        }

        public async Task<bool> DeleteTaskAsync(int id, Guid userId)
        {
            var task = await _context.Tasks.FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);
            if (task == null) return false;

            _context.Tasks.Remove(task);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
