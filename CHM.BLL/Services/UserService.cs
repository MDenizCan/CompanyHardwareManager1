using AutoMapper;
using CHM.BLL.Interfaces;
using CHM.MODELS.User;
using CHM.MODELS.Common;

namespace CHM.BLL.Services;

// Kullanıcılarla ilgili (Login hariç) iş kurallarını (Business Logic) işleten servis.
public sealed class UserService : IUserService
{
    private readonly IUserRepository _users;
    private readonly IMapper _mapper;

    public UserService(IUserRepository users, IMapper mapper)
    {
        _users = users;
        _mapper = mapper;
    }

    public async Task<PagedResponse<UserResponse>> GetAllAsync(PaginationFilter filter, CancellationToken cancellationToken = default)
    {
        var (items, count) = await _users.GetAllAsync(filter, cancellationToken);
        var mapped = _mapper.Map<List<UserResponse>>(items);
        return new PagedResponse<UserResponse>(mapped, count, filter.PageNumber, filter.PageSize);
    }

    public async Task AssignRoleAsync(AssignRoleRequest request, CancellationToken cancellationToken = default)
    {
        //yetki kontrolu yap admin
        await _users.AssignRoleAsync(request.UserId, request.RoleName, cancellationToken);
        await _users.SaveChangesAsync(cancellationToken);
    }
}
