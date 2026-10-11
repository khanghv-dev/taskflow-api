using System;

namespace AuthDemo.Api.DTOs
{
    public class CreateTaskDto
    {
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime Deadline { get; set; }
        public int? CategoryId { get; set; }
    }
}