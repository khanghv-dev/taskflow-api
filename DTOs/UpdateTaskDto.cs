using System;

namespace AuthDemo.Api.DTOs
{
    public class UpdateTaskDto
    {
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime Deadline { get; set; }
        public string Status { get; set; } = "Pending";
        public int? CategoryId { get; set; }
    }
}