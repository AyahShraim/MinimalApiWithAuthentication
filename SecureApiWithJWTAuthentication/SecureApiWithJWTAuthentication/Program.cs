using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SecureApiWithJWTAuthentication.Authentication;
using SecureApiWithJWTAuthentication.DbContexts;
using SecureApiWithJWTAuthentication.Models;
using SecureApiWithJWTAuthentication.Services;
using Serilog;
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
builder.Services.Configure<JwtConfiguration>(options => builder.Configuration.GetSection("JWT"));

var jwtConfiguration = builder.Services.BuildServiceProvider().GetService<JwtConfiguration>();


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
