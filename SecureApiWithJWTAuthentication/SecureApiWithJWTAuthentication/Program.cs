using Microsoft.EntityFrameworkCore;
using SecureApiWithJWTAuthentication.Authentication;
using SecureApiWithJWTAuthentication.DbContexts;
using SecureApiWithJWTAuthentication.Models;
using SecureApiWithJWTAuthentication.Services;
using Serilog;


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
builder.Services.Configure<JwtConfiguration>(options => builder.Configuration.GetSection("JWT"));
builder.Services.AddScoped<IJwtTokenService, JwtTokenGenerator>();

var app = builder.Build();

app.MapGet("/", () => "Hello World!");
app.MapPost("/Login", async (IJwtTokenService tokenService, AuthenticationCredentials authenticationCredentials) =>
{
    var token = await tokenService.GenerateToken(authenticationCredentials);
    if (token == null)
    {
        return Results.Unauthorized();
    }
    return Results.Ok(token);
});

app.Run();

//var jwtConfiguration = builder.Services.BuildServiceProvider().GetService<JwtConfiguration>();

//builder.Services.AddAuthentication("Bearer")
//    .AddJwtBearer(options =>
//    {
//        options.TokenValidationParameters = new()
//        {
//            ValidateIssuer = true,
//            ValidateAudience = true,
//            ValidateIssuerSigningKey = true,
//            ValidIssuer = jwtConfiguration.Issuer,
//            ValidAudience = jwtConfiguration.Audience,
//            IssuerSigningKey = new SymmetricSecurityKey(
//                Encoding.ASCII.GetBytes(jwtConfiguration.Secret))
//        };

//    });