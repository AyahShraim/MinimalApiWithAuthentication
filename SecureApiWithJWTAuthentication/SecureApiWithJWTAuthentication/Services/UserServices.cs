using Microsoft.EntityFrameworkCore;
using SecureApiWithJWTAuthentication.DbContexts;
using SecureApiWithJWTAuthentication.Entities;

namespace SecureApiWithJWTAuthentication.Services
{
    public class UserServices : IUserServices
    {
        private readonly AppDbContext _dbContext;

        public UserServices(AppDbContext dbContext) 
        {
            _dbContext = dbContext;
        }

        public async Task<User?> ValidateUserCredentials(string username, string password)
        {
            return await _dbContext.Users
                .Where(user => user.UserName == username && user.Password == password)
                .FirstOrDefaultAsync();
        }
    }
}
