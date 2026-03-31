namespace CHM.MODELS.Auth;

// Kullanıcı sisteme giriş yapmak (Login) istediğinde gönderdiği istek modeli.
public sealed class LoginRequest
{
    // Kullanıcı adını veya e-posta adresini tutar. İkisi de aynı alandan alınır.
    public string UsernameOrEmail { get; set; } = null!;
    
    // Kullanıcının şifresi
    public string Password { get; set; } = null!;
}
