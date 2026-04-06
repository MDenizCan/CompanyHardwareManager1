using CHM.MODELS.User;
using CHM.MODELS.Common;

namespace CHM.BLL.Interfaces;

public interface IUserService
{
    Task<PagedResponse<UserResponse>> GetAllAsync(PaginationFilter filter, CancellationToken cancellationToken = default);
    Task AssignRoleAsync(AssignRoleRequest request, CancellationToken cancellationToken = default);
    Task RemoveRoleAsync(RemoveRoleRequest request, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
