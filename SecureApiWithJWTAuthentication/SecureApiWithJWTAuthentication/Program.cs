using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SecureApiWithJWTAuthentication.Authentication;
using SecureApiWithJWTAuthentication.DbContexts;
using SecureApiWithJWTAuthentication.Models;
using SecureApiWithJWTAuthentication.Services;
using Serilog;
using System.Security.Claims;
using System.Text;


Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.Console()
    .WriteTo.File("logs/jwtAuthApi.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(
        dbContextOptions => dbContextOptions.UseSqlServer(
            builder.Configuration["ConnectionStrings:UsersApiDbConnection"])
);

builder.Services.AddScoped<IUserServices, UserServices>();
builder.Services.AddSingleton(provider =>
{
    return new SigningConfiguration(builder.Configuration["JWT:Secret"]);

});
builder.Services.Configure<JwtConfiguration>(options => builder.Configuration.GetSection("JWT").Bind(options));
builder.Services.AddScoped<IJwtTokenService, JwtTokenGenerator>();

var jwtConfiguration = builder.Services.BuildServiceProvider().GetService<IOptions<JwtConfiguration>>().Value;
Log.Information($"Issuer: {jwtConfiguration?.Issuer}, Audience: {jwtConfiguration?.Audience}, Secret: {jwtConfiguration?.Secret}");

builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new()
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtConfiguration.Issuer,
            ValidAudience = jwtConfiguration.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.ASCII.GetBytes(jwtConfiguration.Secret))
        };

    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("MustBeFirstMemberInSystem", policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.RequireClaim(ClaimTypes.NameIdentifier, "1");
    });
});

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();


app.MapPost("/Login", async (IJwtTokenService tokenService, AuthenticationCredentials authenticationCredentials) =>
{
    var token = await tokenService.GenerateToken(authenticationCredentials);
    if (token == null)
    {
        return Results.Unauthorized();
    }
    return Results.Ok(token);
});

app.MapGet("/welcome", (ClaimsPrincipal user) =>
{
    var firstName = user.FindFirst(ClaimTypes.GivenName)?.Value;
    var message = $"Hey  {firstName} ! Welcome to the Api :)";
    return Results.Ok(message);
}).RequireAuthorization();

app.MapGet("/specialResource", (ClaimsPrincipal user) =>
{
    return Results.Ok("This is a special resource for the first member in the system.");
}).RequireAuthorization("MustBeFirstMemberInSystem");


app.Run();

