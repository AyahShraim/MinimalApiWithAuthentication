using Microsoft.EntityFrameworkCore;
using SecureApiWithJWTAuthentication.Entities;

namespace SecureApiWithJWTAuthentication.DbContexts
{
    public static class AppDbContextExtensions
    {
        public static void SeedUsers(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasData(
                new User { Id = 1, UserName = "ayahShraim", FirstName = "ayah", LastName = "shraim", Password = "12345" },
                new User { Id = 2, UserName = "regularUser", FirstName = "regular", LastName = "user", Password = "11111" }
            );
        } 
    }
}
