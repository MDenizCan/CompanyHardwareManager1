using CHM.ENTITIES.Entities;
using CHM.MODELS.Common;

namespace CHM.BLL.Interfaces;

public interface IAssignmentRepository
{
    Task<Assignment?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<(List<Assignment> Items, int TotalCount)> GetAllActiveAssignmentsAsync(PaginationFilter filter, CancellationToken cancellationToken = default);
    Task<(List<Assignment> Items, int TotalCount)> GetUserAssignmentsAsync(Guid userId, PaginationFilter filter, CancellationToken cancellationToken = default);
    Task AddAsync(Assignment assignment, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
