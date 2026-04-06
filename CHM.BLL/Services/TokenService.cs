using CHM.BLL.Interfaces;
using CHM.ENTITIES.Entities;
using CHM.MODELS.Auth;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace CHM.BLL.Services;

// JWT (JSON Web Token) kimlik doğrulama anahtarlarını oluşturan servis.
public sealed class TokenService : ITokenService
{
    private readonly JwtOptions _jwt;

    public TokenService(IOptions<JwtOptions> jwtOptions)
    {
        _jwt = jwtOptions.Value;
    }

    // Başarılı bir giriş sonrası hem kısan ömürlü (Access) hem de uzun ömürlü (Refresh) token çiftini oluşturur.
    public TokenResponse CreateTokenPair(User user)
    {
        var now = DateTime.UtcNow;
        var accessExpiresAt = now.AddMinutes(_jwt.AccessTokenMinutes);
        var refreshExpiresAt = now.AddDays(_jwt.RefreshTokenDays);

        var accessToken = CreateJwt(user, accessExpiresAt);
        var refreshToken = GenerateRefreshToken();

        return new TokenResponse
        {
            AccessToken = accessToken,
            AccessTokenExpiresAt = accessExpiresAt,
            RefreshToken = refreshToken,
            RefreshTokenExpiresAt = refreshExpiresAt,
            UserId = user.Id,
            Username = user.Username,
            Email = user.Email,
            Roles = user.UserRoles.Select(ur => ur.Role.Name.ToString()).ToList()
        };
    }

    public RefreshToken CreateRefreshToken(Guid userId, int refreshTokenDays)
    {
        return new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Token = GenerateRefreshToken(),
            ExpiresAt = DateTime.UtcNow.AddDays(refreshTokenDays),
            CreatedAt = DateTime.UtcNow
        };
    }

    // JWT (Access Token) üretir. İçerisinde (Payload) kullanıcının ID'sini, rollerini ve e-postasını taşır (Claims).
    private string CreateJwt(User user, DateTime expiresAt)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(JwtRegisteredClaimNames.UniqueName, user.Username),
            new(JwtRegisteredClaimNames.Email, user.Email),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("N"))
        };

        foreach (var roleName in user.UserRoles.Select(ur => ur.Role.Name.ToString()).Distinct(StringComparer.OrdinalIgnoreCase))
        {
            claims.Add(new Claim(ClaimTypes.Role, roleName));
        }

        var token = new JwtSecurityToken(
            issuer: _jwt.Issuer,
            audience: _jwt.Audience,
            claims: claims,
            notBefore: DateTime.UtcNow,
            expires: expiresAt,
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    // Veritabanında (RefreshTokens tablosunda) tutulmak üzere kriptografik olarak güvenli ve rastgele bir metin (Token) üretir.
    private static string GenerateRefreshToken()
    {
        // 64 bytes -> 86 chars base64 (approx); OK for storage
        var bytes = RandomNumberGenerator.GetBytes(64);
        return Convert.ToBase64String(bytes);
    }
}

