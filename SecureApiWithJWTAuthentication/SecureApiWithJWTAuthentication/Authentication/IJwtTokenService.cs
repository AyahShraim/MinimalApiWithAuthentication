using SecureApiWithJWTAuthentication.Models;

namespace SecureApiWithJWTAuthentication.Authentication
{
    public interface IJwtTokenService
    {
        Task<string> GenerateToken(AuthenticationCredentials authenticationCredentials);
        Task<bool> VerifyToken(string token);
    }
}
