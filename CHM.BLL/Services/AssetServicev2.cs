using AutoMapper;
using CHM.BLL.Interfaces;
using CHM.ENTITIES.Entities;
using CHM.MODELS.Asset;
using CHM.MODELS.Common;

namespace CHM.BLL.Services;

// Cihazlarla (Asset) ilgili tüm iş kurallarının (Business Logic) işletildiği ana servis.
// Controller'lar doğrudan Repository ile konuşmaz, bu servis aracılığıyla veriye erişir.
public sealed class AssetServicev2 : IAssetService
{
    private readonly IAssetRepository _assets;
    private readonly IMapper _mapper;

    public AssetService(IAssetRepository assets, IMapper mapper)
    {
        _assets = assets;
        _mapper = mapper;
    }

    // Verilen ID'ye göre cihazı bulur, bulamazsa hata fırlatır ve DTO'ya (AssetResponse) çevirerek döner.
    public async Task<AssetResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var asset = await _assets.GetByIdAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException($"Asset with id '{id}' not found.");

        return _mapper.Map<AssetResponse>(asset);
    }

    // Tüm cihazları DTO modeline (AssetListResponse) çevirerek gönderir. Pagination içerir.
    public async Task<PagedResponse<AssetListResponse>> GetAllAsync(PaginationFilter filter, CancellationToken cancellationToken = default)
    {
        var (items, totalCount) = await _assets.GetAllAsync(filter, cancellationToken);
        var mappedData = _mapper.Map<List<AssetListResponse>>(items);
        return new PagedResponse<AssetListResponse>(mappedData, totalCount, filter.PageNumber, filter.PageSize);
    }

    // Yeni cihaz eklemek için gerekli iş kurallarını işletir (Örn: Benzersiz Seri Numarası kontrolü).
    public async Task<AssetResponse> CreateAsync(CreateAssetRequest request, CancellationToken cancellationToken = default)
    {
        if (await _assets.SerialNumberExistsAsync(request.SerialNumber.Trim(), cancellationToken: cancellationToken))
            throw new InvalidOperationException($"Serial number '{request.SerialNumber}' already exists.");

        var asset = _mapper.Map<Asset>(request);
        asset.Id = Guid.NewGuid();
        asset.Status = AssetStatus.Available;
        asset.CreatedAt = DateTime.UtcNow;

        await _assets.AddAsync(asset, cancellationToken);
        await _assets.SaveChangesAsync(cancellationToken);

        return _mapper.Map<AssetResponse>(asset);
    }

    // Mevcut bir cihazı günceller. Yine seri numarası çakışmalarını ve uygun durum (Enum) değerlerini kontrol eder.
    public async Task<AssetResponse> UpdateAsync(Guid id, UpdateAssetRequest request, CancellationToken cancellationToken = default)
    {
        var asset = await _assets.GetByIdAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException($"Asset with id '{id}' not found.");

        if (!Enum.IsDefined(typeof(AssetStatus), request.Status))
            throw new InvalidOperationException($"Invalid status value: {request.Status}.");

        // Check serial number uniqueness (exclude current asset)
        if (await _assets.SerialNumberExistsAsync(request.SerialNumber.Trim(), excludeId: id, cancellationToken: cancellationToken))
            throw new InvalidOperationException($"Serial number '{request.SerialNumber}' already exists.");

        asset.Name = request.Name.Trim();
        asset.SerialNumber = request.SerialNumber.Trim();
        asset.Description = request.Description?.Trim();
        asset.Status = (AssetStatus)request.Status;
        asset.CategoryId = request.CategoryId;
        asset.UpdatedAt = DateTime.UtcNow;

        await _assets.SaveChangesAsync(cancellationToken);

        return _mapper.Map<AssetResponse>(asset);
    }

    // Cihazı sistemden siler (Aslında IsDeleted = true yaparak Soft Delete uygular).
    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var asset = await _assets.GetByIdAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException($"Asset with id '{id}' not found.");

        // Soft delete
        asset.IsDeleted = true;
        asset.DeletedAt = DateTime.UtcNow;
        asset.UpdatedAt = DateTime.UtcNow;

        await _assets.SaveChangesAsync(cancellationToken);
    }
}
