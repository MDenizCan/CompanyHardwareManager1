using CHM.BLL.Interfaces;
using CHM.ENTITIES.Entities;
using CHM.MODELS.Common;
using Microsoft.EntityFrameworkCore;

namespace CHM.INFRASTRUCTURE.Repositories;

public sealed class AssignmentRepository : IAssignmentRepository
{
    private readonly AppDbContext _db;

    public AssignmentRepository(AppDbContext db)
    {
        _db = db;
    }

    public Task<Assignment?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return _db.Assignments
            .Include(a => a.Asset)
            .Include(a => a.User)
            .Include(a => a.AssignedByUser)
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
    }

    public async Task<(List<Assignment> Items, int TotalCount)> GetAllActiveAssignmentsAsync(PaginationFilter filter, CancellationToken cancellationToken = default)
    {
        var query = _db.Assignments
            .Include(a => a.Asset)
            .Include(a => a.User)
            .Include(a => a.AssignedByUser)
            .Where(a => a.ReturnedAt == null);

        var count = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderByDescending(a => a.AssignedAt)
            .Skip((filter.PageNumber - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .ToListAsync(cancellationToken);

        return (items, count);
    }

    public async Task<(List<Assignment> Items, int TotalCount)> GetUserAssignmentsAsync(Guid userId, PaginationFilter filter, CancellationToken cancellationToken = default)
    {
        var query = _db.Assignments
            .Include(a => a.Asset)
            .Include(a => a.User)
            .Include(a => a.AssignedByUser)
            .Where(a => a.UserId == userId);

        var count = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderByDescending(a => a.AssignedAt)
            .Skip((filter.PageNumber - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .ToListAsync(cancellationToken);

        return (items, count);
    }

    public async Task AddAsync(Assignment assignment, CancellationToken cancellationToken = default)
    {
        await _db.Assignments.AddAsync(assignment, cancellationToken);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return _db.SaveChangesAsync(cancellationToken);
    }
}
