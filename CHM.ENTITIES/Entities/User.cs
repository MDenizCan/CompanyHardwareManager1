using CHM.ENTITIES.Common;

namespace CHM.ENTITIES.Entities;

// Sistemi kullanan çalışanları veya yöneticileri temsil eden varlık (Entity).
public class User : BaseEntity
{
    // Sisteme giriş yaparken kullanılan kullanıcı adı. Benzersiz (Unique) olmalıdır.
    public string Username { get; set; } = null!;
    
    // Kullanıcının e-posta adresi. İletişim ekleneceğinde kullanılır ve benzersizdir.
    public string Email { get; set; } = null!;
    
    // Güvenlik: Kullanıcının şifresi düz metin olarak değil, şifrelenmiş (Hash) olarak tutulur.
    public string PasswordHash { get; set; } = null!;

    // Kullanıcının hesabının aktif olup olmadığını belirtir. (Örn: İşten ayrıldıysa false yapılır).
    public bool IsActive { get; set; } = true;

    // Kullanıcının ait olduğu departman — FK → Department tablosu
    // Nullable: Yeni kullanıcılar kayıt olduğunda henüz bir departmana atanmamış olabilir.
    public Guid? DepartmentId { get; set; }
    // Navigation Property: EF Core'un ilişkili departman bilgisini (Include ile) getirmesi için kullanılır.
    public Department? Department { get; set; }

    // Navigation Property: Kullanıcının sahip olduğu rollerin listesi (Çoka Çok İlişki için ara tablo).
    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    
    // Navigation Property: Bu kullanıcıya zimmetlenmiş cihazların listesi.
    public ICollection<Assignment> Assignments { get; set; } = new List<Assignment>();
    
    // Navigation Property: Kullanıcının açtığı talepler (Arıza vb.) listesi.
    public ICollection<Request> Requests { get; set; } = new List<Request>();
    
    // Navigation Property: Kullanıcının oturumlarını tazelemek için kullandığı Refresh Token listesi.
    public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
}
