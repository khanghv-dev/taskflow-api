using System.ComponentModel.DataAnnotations;

namespace AuthDemo.Api.DTOs;

/// <summary>
/// DTO dùng khi client gửi yêu cầu tạo danh mục mới.
/// </summary>
public class CreateCategoryDto
{
    // Tên danh mục – bắt buộc, tối đa 50 ký tự
    [Required(ErrorMessage = "Tên danh mục là bắt buộc.")]
    [StringLength(50, ErrorMessage = "Tên danh mục không được vượt quá 50 ký tự.")]
    public string Name { get; set; } = string.Empty;

    // Mô tả danh mục – không bắt buộc
    public string? Description { get; set; }
}
