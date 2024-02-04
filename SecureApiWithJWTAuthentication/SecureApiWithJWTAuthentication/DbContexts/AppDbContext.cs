using Microsoft.EntityFrameworkCore;
using SecureApiWithJWTAuthentication.Entities;
namespace SecureApiWithJWTAuthentication.DbContexts
{
    public class AppDbContext : DbContext
    {
        public DbSet<User> Users { get; set; } = null!;

        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        { 

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.SeedUsers();
        }
    }
}
