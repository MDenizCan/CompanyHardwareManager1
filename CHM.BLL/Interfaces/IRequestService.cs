using CHM.MODELS.Request;
using CHM.MODELS.Common;

namespace CHM.BLL.Interfaces;

public interface IRequestService
{
    Task<RequestResponseDto> CreateRequestAsync(Guid userId, CreateRequestDto request, CancellationToken cancellationToken = default);
    Task<RequestResponseDto> UpdateStatusAsync(Guid id, UpdateRequestStatusDto request, CancellationToken cancellationToken = default);
    Task<PagedResponse<RequestResponseDto>> GetUserRequestsAsync(Guid userId, PaginationFilter filter, CancellationToken cancellationToken = default);
    Task<PagedResponse<RequestResponseDto>> GetAllAsync(PaginationFilter filter, CancellationToken cancellationToken = default);
}
