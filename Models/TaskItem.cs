using System;
using System.ComponentModel.DataAnnotations;

namespace ManagementApi.Models
{
    public class TaskItem
    {
        public int Id { get; set; }

        [Required]
        public string? Title { get; set; }

        public string? Description { get; set; }

        public DateTime? DueDate { get; set; }

        public int Status { get; set; }
        public int Priority { get; set; }

        // Foreign Key (optional)
        public string? UserId { get; set; }
    }
}
