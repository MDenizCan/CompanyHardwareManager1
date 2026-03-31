using CHM.ENTITIES.Entities;
using CHM.MODELS.Common;

namespace CHM.BLL.Interfaces;

public interface IRequestRepository
{
    Task<Request?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<(List<Request> Items, int TotalCount)> GetUserRequestsAsync(Guid userId, PaginationFilter filter, CancellationToken cancellationToken = default);
    Task<(List<Request> Items, int TotalCount)> GetAllAsync(PaginationFilter filter, CancellationToken cancellationToken = default);
    Task AddAsync(Request request, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
