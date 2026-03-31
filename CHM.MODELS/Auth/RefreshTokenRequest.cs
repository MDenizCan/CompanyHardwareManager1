namespace CHM.MODELS.Auth;

// Access Token'ın süresi dolduğunda, istemcinin (Frontend) tekrar giriş 
// yapmadan yeni bir token istemesi için gönderdiği model.
public sealed class RefreshTokenRequest
{
    // Elindeki aktif (süresi dolmamış) Refresh Token metni.
    public string RefreshToken { get; set; } = null!;
}
