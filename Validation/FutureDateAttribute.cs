using System.ComponentModel.DataAnnotations;

namespace AuthDemo.Api.Validation;

/// <summary>
/// Custom Validation Attribute kiểm tra ngày phải >= thời điểm hiện tại.
/// Dùng cho trường Deadline của TaskItem để đảm bảo không đặt deadline trong quá khứ.
/// Ví dụ sử dụng: [FutureDate(ErrorMessage = "Deadline phải là ngày trong tương lai.")]
/// </summary>
public class FutureDateAttribute : ValidationAttribute
{
    public FutureDateAttribute()
        : base("Ngày phải lớn hơn hoặc bằng thời điểm hiện tại.")
    {
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        // Cho phép giá trị null (trường hợp Deadline không bắt buộc)
        if (value is null)
            return ValidationResult.Success;

        // Kiểm tra giá trị có phải kiểu DateTime không
        if (value is DateTime dateValue)
        {
            if (dateValue >= DateTime.UtcNow)
                return ValidationResult.Success;

            // Ngày trong quá khứ → trả về lỗi
            return new ValidationResult(ErrorMessage ?? "Ngày phải lớn hơn hoặc bằng thời điểm hiện tại.");
        }

        return new ValidationResult("Giá trị không hợp lệ. Yêu cầu kiểu DateTime.");
    }
}
