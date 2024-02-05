using Asp.Versioning;
using Carter;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SecureApiWithJWTAuthentication.Authentication;
using SecureApiWithJWTAuthentication.DbContexts;
using SecureApiWithJWTAuthentication.Models;
using SecureApiWithJWTAuthentication.Services;
using Serilog;
using System.Reflection;
using System.Security.Claims;
using System.Text;


Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.Console()
    .WriteTo.File("logs/jwtAuthApi.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog();

builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1.0);
    options.ReportApiVersions = true;
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ApiVersionReader = new HeaderApiVersionReader("api-version");
});


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(setupAction =>
{
    var xmlCommentsFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlCommentFullPath = Path.Combine(AppContext.BaseDirectory, xmlCommentsFile);
    setupAction.IncludeXmlComments(xmlCommentFullPath);
    setupAction.AddSecurityDefinition("MinimalApiBearerAuth", new OpenApiSecurityScheme()
    {
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        Description = "Input valid token"
    });

    setupAction.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference {
                    Type = ReferenceType.SecurityScheme,
                    Id = "MinimalApiBearerAuth" }
            }, new List<string>() }
    });
});

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

builder.Services.AddCarter();
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
app.MapCarter();

app.Run();

