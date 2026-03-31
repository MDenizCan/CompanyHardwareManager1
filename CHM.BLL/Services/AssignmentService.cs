using AutoMapper;
using CHM.BLL.Interfaces;
using CHM.ENTITIES.Entities;
using CHM.MODELS.Assignment;
using CHM.MODELS.Common;

namespace CHM.BLL.Services;

public sealed class AssignmentService : IAssignmentService
{
    private readonly IAssignmentRepository _assignments;
    private readonly IAssetRepository _assets;
    private readonly IUserRepository _users;
    private readonly IMapper _mapper;

    public AssignmentService(
        IAssignmentRepository assignments,
        IAssetRepository assets,
        IUserRepository users,
        IMapper mapper)
    {
        _assignments = assignments;
        _assets = assets;
        _users = users;
        _mapper = mapper;
    }

    public async Task<AssignmentResponseDto> AssignAssetAsync(AssignmentRequestDto request, Guid assignedByUserId, CancellationToken cancellationToken = default)
    {
        var asset = await _assets.GetByIdAsync(request.AssetId, cancellationToken)
            ?? throw new KeyNotFoundException($"Asset with ID '{request.AssetId}' not found.");

        if (asset.Status != AssetStatus.Available)
            throw new InvalidOperationException($"Asset is not available for assignment. Current status: {asset.Status}");

        var user = await _users.GetByIdAsync(request.UserId, false, cancellationToken)
            ?? throw new KeyNotFoundException($"User with ID '{request.UserId}' not found.");

        var assignment = new Assignment
        {
            Id = Guid.NewGuid(),
            AssetId = request.AssetId,
            UserId = request.UserId,
            AssignedByUserId = assignedByUserId,
            AssignedAt = DateTime.UtcNow
        };

        // Update Asset status
        asset.Status = AssetStatus.InUse;
        asset.UpdatedAt = DateTime.UtcNow;

        await _assignments.AddAsync(assignment, cancellationToken);
        await _assignments.SaveChangesAsync(cancellationToken);

        // Fetch back from DB to get included data for DTO mapping
        var savedAssignment = await _assignments.GetByIdAsync(assignment.Id, cancellationToken);
        return _mapper.Map<AssignmentResponseDto>(savedAssignment!);
    }

    public async Task<AssignmentResponseDto> ReturnAssetAsync(ReturnAssetDto request, CancellationToken cancellationToken = default)
    {
        var assignment = await _assignments.GetByIdAsync(request.AssignmentId, cancellationToken)
            ?? throw new KeyNotFoundException($"Assignment with ID '{request.AssignmentId}' not found.");

        if (assignment.ReturnedAt.HasValue)
            throw new InvalidOperationException("This asset has already been returned.");

        if (!Enum.IsDefined(typeof(AssetStatus), request.ReturnStatus))
            throw new InvalidOperationException($"Invalid return status value: {request.ReturnStatus}");

        var asset = await _assets.GetByIdAsync(assignment.AssetId, cancellationToken)
            ?? throw new KeyNotFoundException($"Asset with ID '{assignment.AssetId}' not found.");

        // Mark assignment as returned
        assignment.ReturnedAt = DateTime.UtcNow;
        assignment.UpdatedAt = DateTime.UtcNow;

        // Update asset status
        asset.Status = (AssetStatus)request.ReturnStatus;
        asset.UpdatedAt = DateTime.UtcNow;
        
        await _assignments.SaveChangesAsync(cancellationToken);

        return _mapper.Map<AssignmentResponseDto>(assignment);
    }

    public async Task<PagedResponse<AssignmentResponseDto>> GetUserAssignmentsAsync(Guid userId, PaginationFilter filter, CancellationToken cancellationToken = default)
    {
        var (items, count) = await _assignments.GetUserAssignmentsAsync(userId, filter, cancellationToken);
        var mapped = _mapper.Map<List<AssignmentResponseDto>>(items);
        return new PagedResponse<AssignmentResponseDto>(mapped, count, filter.PageNumber, filter.PageSize);
    }

    public async Task<PagedResponse<AssignmentResponseDto>> GetAllActiveAssignmentsAsync(PaginationFilter filter, CancellationToken cancellationToken = default)
    {
        var (items, count) = await _assignments.GetAllActiveAssignmentsAsync(filter, cancellationToken);
        var mapped = _mapper.Map<List<AssignmentResponseDto>>(items);
        return new PagedResponse<AssignmentResponseDto>(mapped, count, filter.PageNumber, filter.PageSize);
    }
}
