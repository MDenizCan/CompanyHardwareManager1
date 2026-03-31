using CHM.BLL.Interfaces;
using CHM.ENTITIES.Entities;
using CHM.MODELS.Common;
using Microsoft.EntityFrameworkCore;

namespace CHM.INFRASTRUCTURE.Repositories;

public sealed class RequestRepository : IRequestRepository
{
    private readonly AppDbContext _db;

    public RequestRepository(AppDbContext db)
    {
        _db = db;
    }

    public Task<Request?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return _db.Requests
            .Include(r => r.User)
            .Include(r => r.Asset)
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
    }

    public async Task<(List<Request> Items, int TotalCount)> GetUserRequestsAsync(Guid userId, PaginationFilter filter, CancellationToken cancellationToken = default)
    {
        var query = _db.Requests
            .Include(r => r.User)
            .Include(r => r.Asset)
            .Where(r => r.UserId == userId);

        var count = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderByDescending(r => r.CreatedAt)
            .Skip((filter.PageNumber - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .ToListAsync(cancellationToken);

        return (items, count);
    }

    public async Task<(List<Request> Items, int TotalCount)> GetAllAsync(PaginationFilter filter, CancellationToken cancellationToken = default)
    {
        var query = _db.Requests
            .Include(r => r.User)
            .Include(r => r.Asset);

        var count = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderByDescending(r => r.CreatedAt)
            .Skip((filter.PageNumber - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .ToListAsync(cancellationToken);

        return (items, count);
    }

    public async Task AddAsync(Request request, CancellationToken cancellationToken = default)
    {
        await _db.Requests.AddAsync(request, cancellationToken);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return _db.SaveChangesAsync(cancellationToken);
    }
}
