using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using UserManagementAPI.Data;
using UserManagementAPI.Helper;
using UserManagementAPI.Middleware;
using UserManagementAPI.Models;
using UserManagementAPI.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllers();

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

// Register EF-backed repository
builder.Services.AddScoped<IUserRepository, UserRepository>();

builder.Services.AddTransient<ErrorHandlingMiddleware>();
builder.Services.AddTransient<TokenAuthenticationMiddleware>();
builder.Services.AddTransient<RequestLoggingMiddleware>();


var app = builder.Build();

// Middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
// Correct order: error ? auth ? logging
app.UseMiddleware<ErrorHandlingMiddleware>();
app.UseMiddleware<TokenAuthenticationMiddleware>();
app.UseMiddleware<RequestLoggingMiddleware>();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();