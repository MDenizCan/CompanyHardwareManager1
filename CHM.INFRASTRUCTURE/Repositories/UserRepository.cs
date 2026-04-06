using CHM.BLL.Interfaces;
using CHM.ENTITIES.Entities;
using CHM.MODELS.Common;
using Microsoft.EntityFrameworkCore;

namespace CHM.INFRASTRUCTURE.Repositories;

// Kullanıcılarla ilgili veritabanı sorgularının yapıldığı veri erişim sınıfı.
public sealed class UserRepository : IUserRepository
{
    private readonly AppDbContext _db;

    public UserRepository(AppDbContext db)
    {
        _db = db;
    }

    // Sistemdeki tüm kullanıcıları ve rollerini getirir (Admin panelinde vs. listelemek için).
    public async Task<(List<User> Items, int TotalCount)> GetAllAsync(PaginationFilter filter, CancellationToken cancellationToken = default)
    {
        var query = _db.Users
            .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role);

        var count = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderByDescending(u => u.CreatedAt)
            .Skip((filter.PageNumber - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .ToListAsync(cancellationToken);

        return (items, count);
    }

    // ID'ye göre kullanıcıyı bulur. includeRoles true ise kullanıcının rollerini de (Join yaparak) getirir.
    public Task<User?> GetByIdAsync(Guid id, bool includeRoles, CancellationToken cancellationToken = default)
    {
        IQueryable<User> q = _db.Users;
        if (includeRoles)
        {
            q = q.Include(u => u.UserRoles).ThenInclude(ur => ur.Role);
        }

        return q.FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
    }

    // Giriş (Login) senaryosu için kullanıcıyı e-posta VEYA kullanıcı adıyla bulur.
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

    // Kayıt olurken kullanılmak istenen kullanıcı adının sistemde var olup olmadığını (Unique kontrol) test eder.
    public Task<bool> UsernameExistsAsync(string username, CancellationToken cancellationToken = default)
        => _db.Users.AnyAsync(u => u.Username == username, cancellationToken);

    // Kayıt olurken kullanılmak istenen e-posta adresinin var olup olmadığını test eder.
    public Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default)
        => _db.Users.AnyAsync(u => u.Email == email, cancellationToken);

    // Yeni kullanıcıyı doğrudan veritabanına ekler (Rol ataması yapmaz).
    public async Task AddAsync(User user, CancellationToken cancellationToken = default)
    {
        await _db.Users.AddAsync(user, cancellationToken);
    }

    // Yeni kullanıcıyı, belirtilen rol (Örn: "User" veya "Admin") ile birlikte veritabanına ekler.
    public async Task AddWithRoleAsync(User user, string roleName, CancellationToken cancellationToken = default)
    {
        // Önce veritabanından atanmak istenen rolün var olup olmadığına bakar.
        if (!Enum.TryParse<RoleType>(roleName, true, out var parsedRole))
            throw new InvalidOperationException($"Invalid role '{roleName}'.");

        var role = await _db.Roles.FirstOrDefaultAsync(r => r.Name == parsedRole, cancellationToken);
        if (role is null)
            throw new InvalidOperationException($"Role '{roleName}' not found. Seed roles first.");

        // Kullanıcıya bu rolü ara tablo (UserRole) üzerinden tanımlar.
        user.UserRoles.Add(new UserRole
        {
            UserId = user.Id,
            RoleId = role.Id,
            Role = role,
            User = user
        });

        // Kullanıcıyı (içindeki rollerle beraber) veritabanına ekler.
        await _db.Users.AddAsync(user, cancellationToken);
    }

    // Mevcut bir kullanıcıya yeni bir rol ekler. (Örn: Employee kullanıcısına Admin yetkisi vermek)
    public async Task AssignRoleAsync(Guid userId, string roleName, CancellationToken cancellationToken = default)
    {
        var user = await _db.Users
            .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

        if (user is null)
            throw new InvalidOperationException($"Kullanıcı (ID: {userId}) bulunamadı.");

        if (!Enum.TryParse<RoleType>(roleName, true, out var parsedRole))
            throw new InvalidOperationException($"Invalid role '{roleName}'.");

        var role = await _db.Roles.FirstOrDefaultAsync(r => r.Name == parsedRole, cancellationToken);
        if (role is null)
            throw new InvalidOperationException($"'{roleName}' isminde bir rol bulunamadı. Lütfen Seed işlemini kontrol edin.");

        // Kullanıcının bu role zaten sahip olup olmadığını kontrol et
        if (!user.UserRoles.Any(ur => ur.RoleId == role.Id))
        {
            user.UserRoles.Add(new UserRole
            {
                UserId = user.Id,
                RoleId = role.Id,
                Role = role,
                User = user
            });
        }
    }

    // Mevcut bir kullanıcıdan belirtilen rolü kaldırır.
    public async Task RemoveRoleAsync(Guid userId, string roleName, CancellationToken cancellationToken = default)
    {
        var user = await _db.Users
            .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

        if (user is null)
            throw new KeyNotFoundException($"Kullanıcı (ID: {userId}) bulunamadı.");

        if (!Enum.TryParse<RoleType>(roleName, true, out var parsedRole))
            throw new InvalidOperationException($"Geçersiz rol: '{roleName}'.");

        var userRole = user.UserRoles.FirstOrDefault(ur => ur.Role.Name == parsedRole);
        if (userRole is null)
            throw new InvalidOperationException($"Kullanıcının '{roleName}' rolü bulunmuyor.");

        // Kullanıcının en az bir rolü kalmalı
        if (user.UserRoles.Count <= 1)
            throw new InvalidOperationException("Kullanıcının en az bir rolü olmalıdır. Son rol kaldırılamaz.");

        user.UserRoles.Remove(userRole);
    }

    // Kullanıcıyı veritabanından kalıcı olarak siler.
    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
        if (user is null)
            throw new KeyNotFoundException($"Kullanıcı (ID: {id}) bulunamadı.");

        _db.Users.Remove(user);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
        => _db.SaveChangesAsync(cancellationToken);
}
