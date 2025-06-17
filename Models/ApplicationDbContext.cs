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
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // modelBuilder.Entity<User>()
            //     .HasMany(u => u.Tasks)
            //     .WithOne(t => t.User)
            //     .HasForeignKey(t => t.UserId)
            //     .OnDelete(DeleteBehavior.Cascade); // Ensures tasks are deleted when user is deleted

            // modelBuilder.Entity<TaskItem>()
            //     .Property(t => t.Status)
            //     .HasConversion<int>(); // Converts Status enum to int for storage

            // modelBuilder.Entity<TaskItem>()
            //     .Property(t => t.Priority)
            //     .HasConversion<int>(); // Converts Priority enum to int for storage

            modelBuilder.Entity<TaskItem>()
                .HasOne(t => t.User) // Navigation property
                .WithMany(u => u.Tasks) // Reverse navigation
                .HasForeignKey(t => t.UserId); 
        }
    }
}
