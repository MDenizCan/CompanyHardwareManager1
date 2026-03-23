using CHM.ENTITIES.Entities;

namespace CHM.BLL.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(Guid id, bool includeRoles, CancellationToken cancellationToken = default);
    Task<User?> GetByUsernameOrEmailAsync(string usernameOrEmail, bool includeRoles, CancellationToken cancellationToken = default);
    Task<bool> UsernameExistsAsync(string username, CancellationToken cancellationToken = default);
    Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default);

    Task AddAsync(User user, CancellationToken cancellationToken = default);
    Task AddWithRoleAsync(User user, string roleName, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}

