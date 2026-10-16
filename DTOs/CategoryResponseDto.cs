namespace AuthDemo.Api.DTOs;

/// <summary>
/// DTO trả về thông tin Category cho client.
/// Chỉ chứa các trường cần thiết, không expose navigation property.
/// </summary>
public class CategoryResponseDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
}
