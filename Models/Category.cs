using System.ComponentModel.DataAnnotations;

namespace AuthDemo.Api.Models;

/// <summary>
/// Entity đại diện cho danh mục công việc (Category).
/// Mỗi Category có thể chứa nhiều TaskItem (quan hệ 1-N).
/// </summary>
public class Category
{
    public int Id { get; set; }

    // Tên danh mục – bắt buộc, tối đa 50 ký tự
    [Required]
    [StringLength(50)]
    public string Name { get; set; } = string.Empty;

    // Mô tả danh mục – không bắt buộc
    public string? Description { get; set; }

    // Navigation property: một Category chứa nhiều TaskItem
    public ICollection<TaskItem> Tasks { get; set; } = new List<TaskItem>();
}
