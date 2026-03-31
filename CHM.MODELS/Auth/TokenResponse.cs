namespace CHM.MODELS.Auth;

// Başarılı bir Login veya Token Yenileme işleminden sonra API'nin istemciye (kullanıcıya) döndüğü veri.
public sealed class TokenResponse
{
    // Her API isteğinde (Bearer Token) kullanılması gereken kısa ömürlü anahtar.
    public string AccessToken { get; set; } = null!;
    public DateTime AccessTokenExpiresAt { get; set; }

    // Access Token bittiğinde yenisini almak için kullanılan uzun ömürlü anahtar.
    // Çok tehlikelidir, HTTP Only Cookielerde veya çok güvenli yerlerde saklanmalıdır.
    public string RefreshToken { get; set; } = null!;
    public DateTime RefreshTokenExpiresAt { get; set; }
}
