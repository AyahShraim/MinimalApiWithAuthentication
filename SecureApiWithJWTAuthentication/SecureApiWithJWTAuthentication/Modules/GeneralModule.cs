﻿using Asp.Versioning;
using Carter;
using System.Security.Claims;

namespace SecureApiWithJWTAuthentication.Modules
{
    public class GeneralModule : ICarterModule
    {

        public void AddRoutes(IEndpointRouteBuilder app)
        {
            var VersionSet = app.NewApiVersionSet()
                                .HasApiVersion(new ApiVersion(1.0))
                                .ReportApiVersions()
                                .Build();

            app.MapGet("/welcome", (ClaimsPrincipal user) =>
            {
                var firstName = user.FindFirst(ClaimTypes.GivenName)?.Value;
                var lastName = user.FindFirst(ClaimTypes.Surname)?.Value;
                var message = $"Hey  {firstName} {lastName} ! Welcome to the Api :)";
                return Results.Ok(message);
            }).RequireAuthorization()
            .WithApiVersionSet(VersionSet)
            .MapToApiVersion(new ApiVersion(1.0));

            app.MapGet("/specialResource", (ClaimsPrincipal user) =>
            {
                return Results.Ok("This is a special resource for the first member in the system.");
            }).RequireAuthorization("MustBeFirstMemberInSystem")
            .WithApiVersionSet(VersionSet)
            .MapToApiVersion(new ApiVersion(1.0));
        }
    }
}