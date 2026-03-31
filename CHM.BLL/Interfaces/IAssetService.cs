using CHM.MODELS.Asset;
using CHM.MODELS.Common;

namespace CHM.BLL.Interfaces;

public interface IAssetService
{
    Task<AssetResponse> CreateAsync(CreateAssetRequest request, CancellationToken cancellationToken = default);
    Task<PagedResponse<AssetListResponse>> GetAllAsync(PaginationFilter filter, CancellationToken cancellationToken = default);
    Task<AssetResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<AssetResponse> UpdateAsync(Guid id, UpdateAssetRequest request, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
