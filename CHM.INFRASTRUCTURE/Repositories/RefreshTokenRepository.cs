using CHM.BLL.Interfaces;
using CHM.ENTITIES.Entities;
using Microsoft.EntityFrameworkCore;

namespace CHM.INFRASTRUCTURE.Repositories;

// Kullanıcı oturumlarını yenilemek için kullanılan Refresh Token'ların veritabanı işlemlerini yürüten sınıf.
public sealed class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly AppDbContext _db;

    public RefreshTokenRepository(AppDbContext db)
    {
        _db = db;
    }

    // Yeni üretilen Refresh Token'ı veritabanına eklenmek üzere takip listesine alır.
    public Task AddAsync(RefreshToken refreshToken, CancellationToken cancellationToken = default)
        => _db.RefreshTokens.AddAsync(refreshToken, cancellationToken).AsTask();

    // Verilen Token metnine sahip kaydı bulur. İstenirse token'ın sahibi olan kullanıcıyı (User)
    // ve o kullanıcının rollerini de (Role) beraberinde (Include ile Join yaparak) getirir.
    public Task<RefreshToken?> GetByTokenAsync(string token, bool includeUserAndRoles, CancellationToken cancellationToken = default)
    {
        IQueryable<RefreshToken> q = _db.RefreshTokens;
        if (includeUserAndRoles)
        {
            // Eager Loading: Token'ı çekerken, Kullanıcıyı -> Kullanıcı Roller Ara Tablosunu -> Rolleri de sorguya dahil et.
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

    // İşlemleri fiziksel olarak veritabanına yazar.
    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
        => _db.SaveChangesAsync(cancellationToken);
}
