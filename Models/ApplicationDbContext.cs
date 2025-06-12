using Microsoft.EntityFrameworkCore;

namespace ManagementApi.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        //public DbSet<Student> Students { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<TaskItem> Tasks { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseNpgsql("Your_PostgreSQL_Connection_String");
            }
        }
    }
}
