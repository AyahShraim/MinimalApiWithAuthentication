using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SecureApiWithJWTAuthentication.Entities;
using SecureApiWithJWTAuthentication.Models;
using SecureApiWithJWTAuthentication.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;


namespace SecureApiWithJWTAuthentication.Authentication
{
    public class JwtTokenGenerator : IJwtTokenService
    {
        private readonly IUserServices _userServices;
        private readonly SigningConfiguration _signingConfiguration;
        private readonly JwtConfiguration _jwtConfiguration;
        private readonly ILogger<JwtTokenGenerator> _logger;

        public JwtTokenGenerator(IUserServices userServices, SigningConfiguration signingConfiguration, IOptions<JwtConfiguration> jwtConfiguration, ILogger<JwtTokenGenerator> logger)
        {
            _userServices = userServices;
            _signingConfiguration = signingConfiguration;
            _jwtConfiguration = jwtConfiguration?.Value ?? throw new ArgumentNullException(nameof(jwtConfiguration), "JwtConfiguration must not be null.");
            _logger = logger;
        }

        public async Task<string> GenerateToken(AuthenticationCredentials authenticationCredentials)
        {
            try
            {
                var user = await ValidateUserInfo(authenticationCredentials.UserName, authenticationCredentials.Password);
                if (user == null)
                {
                    return null;
                }

                var claims = new List<Claim>
                {
                    new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new(ClaimTypes.GivenName, user.FirstName),
                    new(ClaimTypes.Surname, user.LastName)
                };

                var jwtSecurityToken = new JwtSecurityToken(
                    _jwtConfiguration.Issuer,
                    _jwtConfiguration.Audience,
                    claims,
                    DateTime.UtcNow,
                    DateTime.UtcNow.AddHours(_jwtConfiguration.TokenExpiryHours),
                    _signingConfiguration.SigningCredentials
                );
                var token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
                return token;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error generating token {ex.Message}");
                return null;
            }
        }

        private async Task<User?> ValidateUserInfo(string? userName, string? password)
        {
            var user = await _userServices.ValidateUserCredentials(userName, password);
            return user;
        }

        public async Task<bool> VerifyToken(string token)
        {
            try
            {
                await Task.Run(() =>
                {
                    var key = Encoding.ASCII.GetBytes(_jwtConfiguration.Secret);
                    var validationParameters = new TokenValidationParameters()
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateIssuerSigningKey = true,
                        ValidateLifetime = true,
                        ValidIssuer = _jwtConfiguration.Issuer,
                        ValidAudience = _jwtConfiguration.Audience,
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                        ClockSkew = TimeSpan.FromSeconds(5)
                    };

                    var tokenHandler = new JwtSecurityTokenHandler();
                    tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);
                });
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Token Validation Error {ex.Message}");
                return false;
            }
        }
    }
}
