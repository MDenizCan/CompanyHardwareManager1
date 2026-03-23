using CHM.MODELS.Asset;

namespace CHM.BLL.Interfaces;

public interface IAssetService
{
    Task<AssetResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<AssetListResponse>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<AssetResponse> CreateAsync(CreateAssetRequest request, CancellationToken cancellationToken = default);
    Task<AssetResponse> UpdateAsync(Guid id, UpdateAssetRequest request, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
