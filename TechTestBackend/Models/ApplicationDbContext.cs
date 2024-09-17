using Microsoft.EntityFrameworkCore;


namespace TechTestBackend.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Image> Images { get; set; }
    }
}

