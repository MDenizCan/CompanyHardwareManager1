using CHM.ENTITIES.Common;

namespace CHM.ENTITIES.Entities;

// Kullanıcının oturumunu yenilemek için kullanılan uzun ömürlü (Refresh Token) nesnesi.
// Access Token süresi dolduğunda, kullanıcıdan tekrar giriş istemeden yeni token almak için kullanılır.
public class RefreshToken : BaseEntity
{
    // Token'ın hangi kullanıcıya ait olduğunu belirten Dış Anahtar (Foreign Key).
    public Guid UserId { get; set; }
    // Navigation Property: İlgili kullanıcı nesnesi.
    public User User { get; set; } = null!;

    // Rastgele oluşturulmuş ve şifrelenmiş token metni.
    public string Token { get; set; } = null!;
    
    // Token'ın son kullanma tarihi. Bu tarihten sonra token ile yeni Access Token alınamaz.
    public DateTime ExpiresAt { get; set; }

    // Token güvenliğe aykırı bir durumda iptal edildiyse (Revoke) bu alana tarih girilir.
    public DateTime? RevokedAt { get; set; }

    // Token'ın hala aktif olup olmadığını gösteren hesaplanmış özellik (Computed Property).
    // Hem iptal edilmemiş (RevokedAt null) olmalı hem de süresi dolmamış olmalıdır.
    public bool IsActive => RevokedAt is null && DateTime.UtcNow < ExpiresAt;
}

