using CHM.ENTITIES.Entities;
using CHM.MODELS.Auth;

namespace CHM.BLL.Interfaces;

public interface ITokenService
{
    TokenResponse CreateTokenPair(User user);
    RefreshToken CreateRefreshToken(Guid userId, int refreshTokenDays);
}

