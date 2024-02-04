using Microsoft.IdentityModel.Tokens;
using SecureApiWithJWTAuthentication.Entities;
using SecureApiWithJWTAuthentication.Models;
using SecureApiWithJWTAuthentication.Services;
using System.Security.Claims;
using System.Text;

namespace SecureApiWithJWTAuthentication.Authentication
{
    public class JwtTokenGenerator : IJwtTokenService
    {
        private readonly IUserServices _userServices;
        private readonly IConfiguration _config;

        public JwtTokenGenerator(IUserServices userServices, IConfiguration config)
        {
            _userServices = userServices;
            _config = config ?? throw new ArgumentNullException(nameof(config));
        }


        public string GenerateToken(AuthenticationCredentials authenticationCredentials)
        {
            var user = ValidateUserInfo(authenticationCredentials.UserName, authenticationCredentials.Password);
            if (user == null)
            {
                return null;
            }
            var securityKey = new SymmetricSecurityKey(
                Encoding.ASCII.GetBytes(_config["JWT:Secret"]));
            var signingCredentials = new SigningCredentials(
                securityKey, SecurityAlgorithms.HmacSha256);
            var claims = new List<Claim>();
            claims.Add(new Claim("sub", user.Id.ToString()));
      
                
            


        }


        public bool VerifyToken(string token)
        {
            throw new NotImplementedException();
        }

        private async Task<User?> ValidateUserInfo(string? userName, string? password)
        {
            var user = await _userServices.ValidateUserCredentials(userName, password);
            return user;
        }   
    }
}
