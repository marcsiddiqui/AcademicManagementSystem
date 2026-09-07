using Microsoft.EntityFrameworkCore;

namespace AcademicManagementSystem.DatabaseConfiguration
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Department> Departments { get; set; }
        public DbSet<Course> Courses { get; set; }
        // Define your DbSets for your entities here
        // Example:
        // public DbSet<Student> Students { get; set; }
        // public DbSet<Course> Courses { get; set; }
    }
}
