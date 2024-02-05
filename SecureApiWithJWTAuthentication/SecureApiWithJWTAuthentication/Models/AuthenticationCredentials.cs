namespace SecureApiWithJWTAuthentication.Models
{

    /// <summary>
    /// Represents the credentials for user authentication 
    /// </summary>
    public class AuthenticationCredentials
    {
        /// <summary>
        /// the user name for authentication the user
        /// </summary>
        public string? UserName { get; set; }

        /// <summary>
        /// the password for authentication the user
        /// </summary>
        public string? Password { get; set; }
    }
}
