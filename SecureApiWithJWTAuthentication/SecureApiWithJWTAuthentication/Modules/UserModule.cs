using Asp.Versioning;
using Carter;
using SecureApiWithJWTAuthentication.Authentication;
using SecureApiWithJWTAuthentication.Models;

namespace SecureApiWithJWTAuthentication.Modules
{
    /// <summary>
    /// Module for handling user related endpoints
    /// </summary>
    
    public class UserModule : ICarterModule
    {
  
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            var VersionSet = app.NewApiVersionSet()
                        .HasApiVersion(new ApiVersion(1.0))
                        .ReportApiVersions()
                        .Build();

            /// <summary>
            /// End point for user login and token generation
            /// <param name="tokenService">Service for JWT token generation</param>
            /// <param name="authenticationCredentials">user authentication Credentials</param>
            /// <returns>Returns an HTTP Unauthorized result if token generation fails; otherwise, returns an HTTP OK result with the generated token.</returns>
            /// </summary>
            app.MapPost("/Login", async (IJwtTokenService tokenService, AuthenticationCredentials authenticationCredentials) =>
            {
                var token = await tokenService.GenerateToken(authenticationCredentials);
                if (token == null)
                {
                    return Results.Unauthorized();
                }
                return Results.Ok(token);
            }).WithApiVersionSet(VersionSet)
            .MapToApiVersion(new ApiVersion(1.0));
        }
    }
}
