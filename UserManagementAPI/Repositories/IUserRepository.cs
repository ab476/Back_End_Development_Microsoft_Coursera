using UserManagementAPI.Models;

namespace UserManagementAPI.Repositories;

public interface IUserRepository
{
    Task<IEnumerable<User>> GetAllAsync(CancellationToken ct);
    Task<User?> GetByIdAsync(Guid id, CancellationToken ct);
    Task CreateAsync(User user, CancellationToken ct);
    Task UpdateAsync(User user, CancellationToken ct);
    Task DeleteAsync(Guid id, CancellationToken ct);
}

