using System;
using System.ComponentModel.DataAnnotations;

namespace AuthDemo.Api.Models
{
    public class TaskItem
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }

        public DateTime Deadline { get; set; }

        [MaxLength(50)]
        public string Status { get; set; } = "Pending";

        [Required]
        public Guid UserId { get; set; } // Khớp với Guid của User

        public int? CategoryId { get; set; }
    }
}