using AutoMapper;
using CHM.BLL.Interfaces;
using CHM.ENTITIES.Entities;
using CHM.MODELS.Request;
using CHM.MODELS.Common;

namespace CHM.BLL.Services;

public sealed class RequestService : IRequestService
{
    private readonly IRequestRepository _requests;
    private readonly IAssetRepository _assets;
    private readonly IMapper _mapper;

    public RequestService(
        IRequestRepository requests,
        IAssetRepository assets,
        IMapper mapper)
    {
        _requests = requests;
        _assets = assets;
        _mapper = mapper;
    }

    public async Task<RequestResponseDto> CreateRequestAsync(Guid userId, CreateRequestDto request, CancellationToken cancellationToken = default)
    {
        if (!Enum.IsDefined(typeof(RequestType), request.Type))
            throw new InvalidOperationException($"Invalid request type: {request.Type}");

        var type = (RequestType)request.Type;

        var asset = await _assets.GetByIdAsync(request.AssetId, cancellationToken)
            ?? throw new KeyNotFoundException($"Asset with ID '{request.AssetId}' not found.");

        var newReq = new Request
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            AssetId = request.AssetId,
            Type = type,
            Status = RequestStatus.Pending,
            Description = request.Description.Trim(),
            CreatedAt = DateTime.UtcNow
        };

        await _requests.AddAsync(newReq, cancellationToken);
        await _requests.SaveChangesAsync(cancellationToken);

        var savedReq = await _requests.GetByIdAsync(newReq.Id, cancellationToken);
        return _mapper.Map<RequestResponseDto>(savedReq!);
    }

    public async Task<RequestResponseDto> UpdateStatusAsync(Guid id, UpdateRequestStatusDto request, CancellationToken cancellationToken = default)
    {
        var dbReq = await _requests.GetByIdAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException($"Request with ID '{id}' not found.");

        if (!Enum.IsDefined(typeof(RequestStatus), request.Status))
            throw new InvalidOperationException($"Invalid request status: {request.Status}");

        var newStatus = (RequestStatus)request.Status;

        dbReq.Status = newStatus;
        dbReq.UpdatedAt = DateTime.UtcNow;

        await _requests.SaveChangesAsync(cancellationToken);

        return _mapper.Map<RequestResponseDto>(dbReq);
    }

    public async Task<PagedResponse<RequestResponseDto>> GetUserRequestsAsync(Guid userId, PaginationFilter filter, CancellationToken cancellationToken = default)
    {
        var (items, count) = await _requests.GetUserRequestsAsync(userId, filter, cancellationToken);
        var mapped = _mapper.Map<List<RequestResponseDto>>(items);
        return new PagedResponse<RequestResponseDto>(mapped, count, filter.PageNumber, filter.PageSize);
    }

    public async Task<PagedResponse<RequestResponseDto>> GetAllAsync(PaginationFilter filter, CancellationToken cancellationToken = default)
    {
        var (items, count) = await _requests.GetAllAsync(filter, cancellationToken);
        var mapped = _mapper.Map<List<RequestResponseDto>>(items);
        return new PagedResponse<RequestResponseDto>(mapped, count, filter.PageNumber, filter.PageSize);
    }
}
