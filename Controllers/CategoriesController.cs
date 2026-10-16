using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AuthDemo.Api.Data;
using AuthDemo.Api.DTOs;
using AuthDemo.Api.Models;

namespace AuthDemo.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriesController : ControllerBase
{
    private readonly AppDbContext _context;

    // Inject AppDbContext thông qua constructor
    public CategoriesController(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// GET /api/categories – Lấy toàn bộ danh mục.
    /// Không yêu cầu xác thực.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var categories = await _context.Categories
            .AsNoTracking()
            .Select(c => new CategoryResponseDto
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description
            })
            .ToListAsync();

        return Ok(new
        {
            success = true,
            message = "Lấy danh sách danh mục thành công.",
            data = categories
        });
    }

    /// <summary>
    /// POST /api/categories – Tạo danh mục mới.
    /// Yêu cầu xác thực JWT (Authorize).
    /// </summary>
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Create([FromBody] CreateCategoryDto dto)
    {
        // ModelState tự động validate [Required], [StringLength] từ DTO
        if (!ModelState.IsValid)
            return BadRequest(new
            {
                success = false,
                message = "Dữ liệu không hợp lệ.",
                data = ModelState
            });

        // Tạo entity Category từ DTO
        var category = new Category
        {
            Name = dto.Name,
            Description = dto.Description
        };

        _context.Categories.Add(category);
        await _context.SaveChangesAsync();

        // Tạo response DTO
        var response = new CategoryResponseDto
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description
        };

        // Trả về 201 Created với location header
        return CreatedAtAction(nameof(GetAll), new { id = category.Id }, new
        {
            success = true,
            message = "Tạo danh mục thành công.",
            data = response
        });
    }
}
