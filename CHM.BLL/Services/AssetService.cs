using AutoMapper;
using CHM.BLL.Interfaces;
using CHM.ENTITIES.Entities;
using CHM.MODELS.Asset;

namespace CHM.BLL.Services;

public sealed class AssetService : IAssetService
{
    private readonly IAssetRepository _assets;
    private readonly IMapper _mapper;

    public AssetService(IAssetRepository assets, IMapper mapper)
    {
        _assets = assets;
        _mapper = mapper;
    }

    public async Task<AssetResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var asset = await _assets.GetByIdAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException($"Asset with id '{id}' not found.");

        return _mapper.Map<AssetResponse>(asset);
    }

    public async Task<List<AssetListResponse>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var assets = await _assets.GetAllAsync(cancellationToken);
        return _mapper.Map<List<AssetListResponse>>(assets);
    }

    public async Task<AssetResponse> CreateAsync(CreateAssetRequest request, CancellationToken cancellationToken = default)
    {
        if (await _assets.SerialNumberExistsAsync(request.SerialNumber.Trim(), cancellationToken: cancellationToken))
            throw new InvalidOperationException($"Serial number '{request.SerialNumber}' already exists.");

        var asset = new Asset
        {
            Id = Guid.NewGuid(),
            Name = request.Name.Trim(),
            SerialNumber = request.SerialNumber.Trim(),
            Description = request.Description?.Trim(),
            Status = AssetStatus.Available,
            CreatedAt = DateTime.UtcNow
        };

        await _assets.AddAsync(asset, cancellationToken);
        await _assets.SaveChangesAsync(cancellationToken);

        return _mapper.Map<AssetResponse>(asset);
    }

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
        asset.UpdatedAt = DateTime.UtcNow;

        await _assets.SaveChangesAsync(cancellationToken);

        return _mapper.Map<AssetResponse>(asset);
    }

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
