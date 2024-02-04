using SecureApiWithJWTAuthentication.Models;

namespace SecureApiWithJWTAuthentication.Authentication
{
    public interface IJwtTokenService
    {
        string GenerateToken(AuthenticationCredentials authenticationCredentials);
        bool VerifyToken(string token);
    }
}
