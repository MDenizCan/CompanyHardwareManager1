using CHM.MODELS.Assignment;
using CHM.MODELS.Common;

namespace CHM.BLL.Interfaces;

public interface IAssignmentService
{
    Task<AssignmentResponseDto> AssignAssetAsync(AssignmentRequestDto request, Guid assignedByUserId, CancellationToken cancellationToken = default);
    Task<AssignmentResponseDto> ReturnAssetAsync(ReturnAssetDto request, CancellationToken cancellationToken = default);
    Task<PagedResponse<AssignmentResponseDto>> GetUserAssignmentsAsync(Guid userId, PaginationFilter filter, CancellationToken cancellationToken = default);
    Task<PagedResponse<AssignmentResponseDto>> GetAllActiveAssignmentsAsync(PaginationFilter filter, CancellationToken cancellationToken = default);
}
