namespace SecureApiWithJWTAuthentication.Models
{
    public class JwtConfiguration
    {
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public string Secret { get; set; } 
        public double TokenExpiryHours { get; set; }
    }
}
