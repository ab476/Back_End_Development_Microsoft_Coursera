using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using UserManagementAPI.Data;
using UserManagementAPI.Helper;
using UserManagementAPI.Middleware;
using UserManagementAPI.Models;
using UserManagementAPI.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllers();
// Secret key for signing tokens
var rawKey = builder.Configuration["Jwt:Key"];
var jwtKey = string.IsNullOrWhiteSpace(rawKey)
    ? Convert.ToBase64String(RandomNumberGenerator.GetBytes(64))
    : rawKey;

var keyBytes = Convert.FromBase64String(jwtKey);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new()
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(keyBytes)
        };
    });

builder.Services.AddValidatorsFromAssemblyContaining<UserValidator>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new()
    {
        Description = "Enter your token like this: Bearer {your-token}",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    options.AddSecurityRequirement(new()
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new()
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// EF Core SQLite
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options
        .UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"))
        .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)
);
builder.Services.AddAuthorizationBuilder()
    .SetFallbackPolicy(new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build());

// Register EF-backed repository
builder.Services.AddScoped<IUserRepository, UserRepository>();

builder.Services.AddTransient<ErrorHandlingMiddleware>();
builder.Services.AddTransient<RequestLoggingMiddleware>();


var app = builder.Build();

// Middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
// Correct order: error → auth → logging
app.UseMiddleware<ErrorHandlingMiddleware>();
app.UseMiddleware<RequestLoggingMiddleware>();

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapPost("/api/token", [AllowAnonymous] (string username, string password) =>
{
    // Simple validation (replace with real user check)
    if (username != "admin" || password != "password")
        return Results.Unauthorized();

    var claims = new[]
    {
        new Claim(ClaimTypes.Name, username),
        new Claim("role", "Admin")
    };

    var token = new JwtSecurityToken(
        claims: claims,
        expires: DateTime.UtcNow.AddHours(1),
        signingCredentials: new SigningCredentials(
            new SymmetricSecurityKey(keyBytes),
            SecurityAlgorithms.HmacSha256)
    );

    var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
    return Results.Ok(new { token = $"Bearer {tokenString}" });
});
app.MapGet("/api/generate-key", () =>
{
    var key = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
    return Results.Ok(new { key });
}).AllowAnonymous();

app.Run();