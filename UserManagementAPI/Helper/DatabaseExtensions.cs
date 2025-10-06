using UserManagementAPI.Data;

namespace UserManagementAPI.Helper;

public static class DatabaseExtensions
{
    public static async Task EnsureDatabaseRecreatedAsync(this IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        await db.Database.EnsureDeletedAsync();
        await db.Database.EnsureCreatedAsync();
    }
}

