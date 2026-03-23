using CHM.BLL.Interfaces;
using CHM.ENTITIES.Entities;
using CHM.MODELS.Auth;
using Microsoft.Extensions.Options;

namespace CHM.BLL.Services;

public sealed class AuthService : IAuthService
{
    private readonly IUserRepository _users;
    private readonly IRefreshTokenRepository _refreshTokens;
    private readonly ITokenService _tokenService;
    private readonly JwtOptions _jwt;

    private const string DefaultEmployeeRoleName = "Employee";

    public AuthService(
        IUserRepository users,
        IRefreshTokenRepository refreshTokens,
        ITokenService tokenService,
        IOptions<JwtOptions> jwtOptions)
    {
        _users = users;
        _refreshTokens = refreshTokens;
        _tokenService = tokenService;
        _jwt = jwtOptions.Value;
    }

    public async Task RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default)
    {
        if (await _users.UsernameExistsAsync(request.Username, cancellationToken))
            throw new InvalidOperationException("Username already exists.");

        if (await _users.EmailExistsAsync(request.Email, cancellationToken))
            throw new InvalidOperationException("Email already exists.");

        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = request.Username.Trim(),
            Email = request.Email.Trim(),
            PasswordHash = PasswordHasher.Hash(request.Password),
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        await _users.AddWithRoleAsync(user, DefaultEmployeeRoleName, cancellationToken);
        await _users.SaveChangesAsync(cancellationToken);
    }

    public async Task<TokenResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        var user = await _users.GetByUsernameOrEmailAsync(request.UsernameOrEmail.Trim(), includeRoles: true, cancellationToken);
        if (user is null)
            throw new InvalidOperationException("Invalid credentials.");

        if (!user.IsActive)
            throw new InvalidOperationException("User is inactive.");

        if (!PasswordHasher.Verify(request.Password, user.PasswordHash))
            throw new InvalidOperationException("Invalid credentials.");

        var refresh = _tokenService.CreateRefreshToken(user.Id, refreshTokenDays: _jwt.RefreshTokenDays);
        await _refreshTokens.AddAsync(refresh, cancellationToken);
        await _refreshTokens.SaveChangesAsync(cancellationToken);

        var accessPair = _tokenService.CreateTokenPair(user);
        accessPair.RefreshToken = refresh.Token;
        accessPair.RefreshTokenExpiresAt = refresh.ExpiresAt;
        return accessPair;
    }

    public async Task<TokenResponse> RefreshAsync(RefreshTokenRequest request, CancellationToken cancellationToken = default)
    {
        var existing = await _refreshTokens.GetByTokenAsync(request.RefreshToken.Trim(), includeUserAndRoles: true, cancellationToken);
        if (existing is null || !existing.IsActive)
            throw new InvalidOperationException("Invalid refresh token.");

        if (!existing.User.IsActive)
            throw new InvalidOperationException("User is inactive.");

        // rotate refresh token
        existing.RevokedAt = DateTime.UtcNow;

        var newRefresh = _tokenService.CreateRefreshToken(existing.UserId, refreshTokenDays: _jwt.RefreshTokenDays);
        await _refreshTokens.AddAsync(newRefresh, cancellationToken);
        await _refreshTokens.SaveChangesAsync(cancellationToken);

        var pair = _tokenService.CreateTokenPair(existing.User);
        pair.RefreshToken = newRefresh.Token;
        pair.RefreshTokenExpiresAt = newRefresh.ExpiresAt;
        return pair;
    }

    public async Task ChangePasswordAsync(Guid userId, ChangePasswordRequest request, CancellationToken cancellationToken = default)
    {
        var user = await _users.GetByIdAsync(userId, includeRoles: false, cancellationToken);
        if (user is null)
            throw new InvalidOperationException("User not found.");

        if (!PasswordHasher.Verify(request.CurrentPassword, user.PasswordHash))
            throw new InvalidOperationException("Current password is incorrect.");

        user.PasswordHash = PasswordHasher.Hash(request.NewPassword);
        user.UpdatedAt = DateTime.UtcNow;

        await _users.SaveChangesAsync(cancellationToken);
    }
}

