using CHM.BLL.Interfaces;
using CHM.ENTITIES.Entities;
using Microsoft.EntityFrameworkCore;

namespace CHM.INFRASTRUCTURE.Repositories;

public sealed class UserRepository : IUserRepository
{
    private readonly AppDbContext _db;

    public UserRepository(AppDbContext db)
    {
        _db = db;
    }

    public Task<User?> GetByIdAsync(Guid id, bool includeRoles, CancellationToken cancellationToken = default)
    {
        IQueryable<User> q = _db.Users;
        if (includeRoles)
        {
            q = q.Include(u => u.UserRoles).ThenInclude(ur => ur.Role);
        }

        return q.FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
    }

    public Task<User?> GetByUsernameOrEmailAsync(string usernameOrEmail, bool includeRoles, CancellationToken cancellationToken = default)
    {
        IQueryable<User> q = _db.Users;
        if (includeRoles)
        {
            q = q.Include(u => u.UserRoles).ThenInclude(ur => ur.Role);
        }

        return q.FirstOrDefaultAsync(u =>
            u.Username == usernameOrEmail || u.Email == usernameOrEmail,
            cancellationToken);
    }

    public Task<bool> UsernameExistsAsync(string username, CancellationToken cancellationToken = default)
        => _db.Users.AnyAsync(u => u.Username == username, cancellationToken);

    public Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default)
        => _db.Users.AnyAsync(u => u.Email == email, cancellationToken);

    public async Task AddAsync(User user, CancellationToken cancellationToken = default)
    {
        await _db.Users.AddAsync(user, cancellationToken);
    }

    public async Task AddWithRoleAsync(User user, string roleName, CancellationToken cancellationToken = default)
    {
        var role = await _db.Roles.FirstOrDefaultAsync(r => r.Name == roleName, cancellationToken);
        if (role is null)
            throw new InvalidOperationException($"Role '{roleName}' not found. Seed roles first.");

        user.UserRoles.Add(new UserRole
        {
            UserId = user.Id,
            RoleId = role.Id,
            Role = role,
            User = user
        });

        await _db.Users.AddAsync(user, cancellationToken);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
        => _db.SaveChangesAsync(cancellationToken);
}

