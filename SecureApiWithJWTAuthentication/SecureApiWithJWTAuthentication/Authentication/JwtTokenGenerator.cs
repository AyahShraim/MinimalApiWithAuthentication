using Microsoft.Extensions.Options;
using SecureApiWithJWTAuthentication.Entities;
using SecureApiWithJWTAuthentication.Models;
using SecureApiWithJWTAuthentication.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;


namespace SecureApiWithJWTAuthentication.Authentication
{
    public class JwtTokenGenerator : IJwtTokenService
    {
        private readonly IUserServices _userServices;
        private readonly SigningConfiguration _signingConfiguration;
        private readonly JwtConfiguration _jwtConfiguration;

        public JwtTokenGenerator(IUserServices userServices, SigningConfiguration signingConfiguration, IOptions<JwtConfiguration> jwtConfiguration)
        {
            _userServices = userServices;
            _signingConfiguration = signingConfiguration;
            _jwtConfiguration = jwtConfiguration.Value;
        }

        public async Task<string> GenerateToken(AuthenticationCredentials authenticationCredentials)
        {
            var user = await ValidateUserInfo(authenticationCredentials.UserName, authenticationCredentials.Password);
            if (user == null)
            {
                return null;
            }

            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new(JwtRegisteredClaimNames.GivenName, user.FirstName),
                new(JwtRegisteredClaimNames.FamilyName, user.LastName)
            };

            var jwtSecurityToken = new JwtSecurityToken(
                _jwtConfiguration.Issuer,
                _jwtConfiguration.Audience,
                claims,
                DateTime.UtcNow,
                DateTime.UtcNow.AddHours(_jwtConfiguration.TokenExpiryHours),
                _signingConfiguration.SigningCredentials
            ) ;
            var token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
            return token;
        }

        private async Task<User?> ValidateUserInfo(string? userName, string? password)
        {
            var user = await _userServices.ValidateUserCredentials(userName, password);
            return user;
        }
        public async Task<bool> VerifyToken(string token)
        {

        }
    }
}
