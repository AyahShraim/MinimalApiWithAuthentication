using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace SecureApiWithJWTAuthentication.Authentication
{
    public class SigningConfiguration
    {
        public SymmetricSecurityKey SymmetricSecurityKey { get; }
        public SigningCredentials SigningCredentials { get; }   
        public SigningConfiguration(string key)
        {
            SymmetricSecurityKey =  new SymmetricSecurityKey(
                Encoding.ASCII.GetBytes(key));

            SigningCredentials = new SigningCredentials(
                SymmetricSecurityKey, SecurityAlgorithms.HmacSha256);
        }
    }
}
