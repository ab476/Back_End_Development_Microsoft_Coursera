using Microsoft.EntityFrameworkCore;
using UserManagementAPI.Data;
using UserManagementAPI.Helper;
using UserManagementAPI.Models;

namespace UserManagementAPI.Repositories;

public class UserRepository(ApplicationDbContext db) : IUserRepository
{
    public async Task<IEnumerable<User>> GetAllAsync(CancellationToken ct)
    {
        return await db.TUsers.AsUserDtoQueryable().ToListAsync(ct);
    }

    public async Task<User?> GetByIdAsync(Guid id, CancellationToken ct)
    {
        var user = await db.TUsers.FindAsync([id], cancellationToken: ct);
        return user?.ToUserDto();
    }

    public async Task CreateAsync(User user, CancellationToken ct)
    {
        await db.TUsers.AddAsync(user.FromUserDto(), ct);
        await db.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(User user, CancellationToken ct)
    {
        await db.TUsers
            .Where(u => u.Id == user.Id)
            .ExecuteUpdateAsync(updates => updates
                .SetProperty(u => u.Name, u => user.Name)
                .SetProperty(u => u.Email, u => user.Email)
                .SetProperty(u => u.Role, u => user.Role),
                ct
            );

    }

    public async Task DeleteAsync(Guid id, CancellationToken ct)
    {
        await db.TUsers
                     .Where(u => u.Id == id)
                     .ExecuteDeleteAsync(ct);

    }
}

