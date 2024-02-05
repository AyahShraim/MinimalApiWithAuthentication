using Asp.Versioning;
using Carter;
using SecureApiWithJWTAuthentication.Authentication;
using SecureApiWithJWTAuthentication.Models;

namespace SecureApiWithJWTAuthentication.Modules
{
    public class UserModule : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            var VersionSet = app.NewApiVersionSet()
                        .HasApiVersion(new ApiVersion(1.0))
                        .ReportApiVersions()
                        .Build();

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
