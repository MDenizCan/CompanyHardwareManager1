namespace CHM.MODELS.Auth;

// Sisteme yeni bir kullanıcı kaydolurken (Register) gönderilen veri modeli.
public sealed class RegisterRequest
{
    // Kullanıcının sisteme girerken kullanacağı kullanıcı adı
    public string Username { get; set; } = null!;
    
    // İletişimde kullanılacak e-mail adresi
    public string Email { get; set; } = null!;
    
    // Kaydolduğu şifresi
    public string Password { get; set; } = null!;
}
