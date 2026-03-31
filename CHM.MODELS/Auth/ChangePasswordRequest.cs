namespace CHM.MODELS.Auth;

// Kullanıcının şifresini değiştirmek istediğinde API'ye gönderdiği model.
public sealed class ChangePasswordRequest
{
    // Güvenlik için önce mevcut şifresini bilmesi gerekir.
    public string CurrentPassword { get; set; } = null!;
    
    // Geçmek istediği yeni şifre.
    public string NewPassword { get; set; } = null!;
}
