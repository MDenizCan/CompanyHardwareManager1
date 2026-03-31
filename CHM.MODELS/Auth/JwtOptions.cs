namespace CHM.MODELS.Auth;

// appsettings.json dosyasından okunan JWT (JSON Web Token) ayarlarını nesneye çevirmek için kullanılan sınıf.
public sealed class JwtOptions
{
    // Token'ı kimin oluşturduğu (Genelde kendi API URL'imiz)
    public string Issuer { get; set; } = null!;
    
    // Token'ı kimlerin kullanabileceği (Genelde kiralayan(tenant) veya kendi frontend URL'imiz)
    public string Audience { get; set; } = null!;
    
    // Token'ı şifrelemek/imzalamak için kullanılan çok gizli anahtar (Private Key)
    public string Key { get; set; } = null!;

    // Access Token'ın ne kadar süre geçerli olacağı (Dakika cinsinden)
    public int AccessTokenMinutes { get; set; } = 15;
    
    // Refresh Token'ın ne kadar süre geçerli olacağı (Gün cinsinden)
    public int RefreshTokenDays { get; set; } = 7;
}
