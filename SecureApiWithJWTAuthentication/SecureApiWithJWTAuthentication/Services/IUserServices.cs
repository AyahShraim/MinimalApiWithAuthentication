using SecureApiWithJWTAuthentication.Entities;

namespace SecureApiWithJWTAuthentication.Services
{
    public interface IUserServices 
    {
        Task<User?> ValidateUserCredentials(string username, string password);
    }
}
