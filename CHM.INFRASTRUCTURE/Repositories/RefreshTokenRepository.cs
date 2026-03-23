using CHM.BLL.Interfaces;
using CHM.ENTITIES.Entities;
using Microsoft.EntityFrameworkCore;

namespace CHM.INFRASTRUCTURE.Repositories;

public sealed class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly AppDbContext _db;

    public RefreshTokenRepository(AppDbContext db)
    {
        _db = db;
    }

    public Task AddAsync(RefreshToken refreshToken, CancellationToken cancellationToken = default)
        => _db.RefreshTokens.AddAsync(refreshToken, cancellationToken).AsTask();

    public Task<RefreshToken?> GetByTokenAsync(string token, bool includeUserAndRoles, CancellationToken cancellationToken = default)
    {
        IQueryable<RefreshToken> q = _db.RefreshTokens;
        if (includeUserAndRoles)
        {
            q = q.Include(rt => rt.User)
                 .ThenInclude(u => u.UserRoles)
                 .ThenInclude(ur => ur.Role);
        }
        else
        {
            q = q.Include(rt => rt.User);
        }

        return q.FirstOrDefaultAsync(rt => rt.Token == token, cancellationToken);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
        => _db.SaveChangesAsync(cancellationToken);
}

