using CHM.BLL.Interfaces;
using CHM.ENTITIES.Entities;
using Microsoft.EntityFrameworkCore;

namespace CHM.INFRASTRUCTURE.Repositories;

public sealed class AssetRepository : IAssetRepository
{
    private readonly AppDbContext _db;

    public AssetRepository(AppDbContext db)
    {
        _db = db;
    }

    public Task<Asset?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => _db.Assets.FirstOrDefaultAsync(a => a.Id == id, cancellationToken);

    public Task<List<Asset>> GetAllAsync(CancellationToken cancellationToken = default)
        => _db.Assets.OrderByDescending(a => a.CreatedAt).ToListAsync(cancellationToken);

    public Task<bool> SerialNumberExistsAsync(string serialNumber, Guid? excludeId = null, CancellationToken cancellationToken = default)
    {
        var query = _db.Assets.Where(a => a.SerialNumber == serialNumber);
        if (excludeId.HasValue)
            query = query.Where(a => a.Id != excludeId.Value);

        return query.AnyAsync(cancellationToken);
    }

    public async Task AddAsync(Asset asset, CancellationToken cancellationToken = default)
        => await _db.Assets.AddAsync(asset, cancellationToken);

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
        => _db.SaveChangesAsync(cancellationToken);
}
