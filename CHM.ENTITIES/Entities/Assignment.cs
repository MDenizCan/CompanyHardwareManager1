using CHM.ENTITIES.Common;

namespace CHM.ENTITIES.Entities;

// Bir cihazın (Asset) bir kullanıcıya (User) atanması işlemini (Zimmet) temsil eden sınıf.
public class Assignment : BaseEntity
{
    // Hangi cihazın atandığını belirten Dış Anahtar (Foreign Key).
    public Guid AssetId { get; set; }
    // Navigation Property: EF Core'un ilişkili cihazı getirmesi (Include) için kullanılır.
    public Asset Asset { get; set; } = null!;

    // Cihazın kime atandığını belirten Dış Anahtar (Foreign Key).
    public Guid UserId { get; set; }
    // Navigation Property: EF Core'un ilişkili kullanıcıyı getirmesi (Include) için kullanılır.
    public User User { get; set; } = null!;

    // Zimmetin ne zaman başladığı. Varsayılan olarak atanma anki UTC zamanı alınır.
    public DateTime AssignedAt { get; set; } = DateTime.UtcNow;
    
    // Cihaz geri iade edildiğinde buraya tarih yazılır. Eğer null ise cihaz hala kullanıcıdadır.
    public DateTime? ReturnedAt { get; set; }

    // İsteğe bağlı (Optional): Bu zimmetleme işlemini gerçekleştiren yönetici veya IT personelinin ID'si.
    public Guid? AssignedByUserId { get; set; }
    // Navigation Property: İşlemi yapan kullanıcı bilgisine erişim sağlar.
    public User? AssignedByUser { get; set; }
}
