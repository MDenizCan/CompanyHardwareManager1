namespace CHM.ENTITIES.Entities;

// Kullanıcı (User) ve Rol (Role) arasındaki Çoka-Çok (Many-to-Many) ilişkiyi 
// veritabanında tutmak için oluşturulmuş ARA TABLO sınıfı.
public class UserRole
{
    // İlişkinin kullanıcı tarafı (Primary Key'in yarısı)
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    // İlişkinin rol tarafı (Primary Key'in diğer yarısı)
    public Guid RoleId { get; set; }
    public Role Role { get; set; } = null!;
}
