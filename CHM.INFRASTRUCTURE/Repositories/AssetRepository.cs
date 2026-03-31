using CHM.BLL.Interfaces;
using CHM.ENTITIES.Entities;
using CHM.MODELS.Common;
using Microsoft.EntityFrameworkCore;

namespace CHM.INFRASTRUCTURE.Repositories;

// Cihazlarla (Asset) ilgili veritabanı işlemlerini yürüten veri erişim sınıfı (Repository Pattern).
// Veritabanı sorgularını (Entity Framework Core) servis katmanından soyutlamak için kullanılır.
public sealed class AssetRepository : IAssetRepository
{
    private readonly AppDbContext _db;

    public AssetRepository(AppDbContext db)
    {
        _db = db;
    }

    // Verilen ID'ye sahip cihazı (eğer silinmemişse) veritabanından bulup getirir. Kategori bilgisini de dahil eder.
    public Task<Asset?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => _db.Assets.Include(a => a.Category).FirstOrDefaultAsync(a => a.Id == id, cancellationToken);

    // Sistemdeki tüm varlıkları veritabanından çekme simülasyonu. Pagination ve Category include eklendi.
    public async Task<(List<Asset> Items, int TotalCount)> GetAllAsync(PaginationFilter filter, CancellationToken cancellationToken = default)
    {
        var query = _db.Assets.Include(a => a.Category).AsQueryable();
        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(a => a.CreatedAt)
            .Skip((filter.PageNumber - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    } // Verilen seri numarasının başka bir cihazda kullanılıp kullanılmadığını kontrol eder (Unique validation için).
    // Güncelleme işlemi sırasında, cihazın kendi ID'si hariç tutulur (excludeId).
    public Task<bool> SerialNumberExistsAsync(string serialNumber, Guid? excludeId = null, CancellationToken cancellationToken = default)
    {
        var query = _db.Assets.Where(a => a.SerialNumber == serialNumber);
        if (excludeId.HasValue)
            query = query.Where(a => a.Id != excludeId.Value);

        return query.AnyAsync(cancellationToken);
    }

    // Yeni oluşturulan cihazı EF Core'un takip listesine ekler (Henüz veritabanına kaydetmez).
    public async Task AddAsync(Asset asset, CancellationToken cancellationToken = default)
        => await _db.Assets.AddAsync(asset, cancellationToken);

    // Yapılan değişiklikleri (Add, Update, Delete) veritabanına fiziksel olarak kaydeder (Commit).
    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
        => _db.SaveChangesAsync(cancellationToken);
}
