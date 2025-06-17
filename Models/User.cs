namespace ManagementApi.Models
{
    public class User
    {
        public int Id { get; set; } // Primary Key
        public string? Username { get; set; }
        public string? PasswordHash { get; set; } // Store hashed passwords for security
        public string? Email { get; set; }
        public string? Role { get; set; } = "User"; // Default role
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public ICollection<TaskItem> Tasks { get; set; } = new List<TaskItem>(); // Navigation property for related tasks
    }
}