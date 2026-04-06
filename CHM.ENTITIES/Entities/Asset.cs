using CHM.ENTITIES.Common;

namespace CHM.ENTITIES.Entities;

// Cihazın mevcut durumunu belirten Enum (Numaralandırma) yapısı.
public enum AssetStatus
{
    Available,          // Kullanıma hazır / Boşta
    InUse,              // Şu anda kullanımda / Birine atanmış
    Faulty,             // Arızalı
    UnderMaintenance,   // Bakımda
    Retired             // Emekliye ayrılmış / Kullanım dışı
}

    // Şirket envanterindeki donanımları (Cihaz, Bilgisayar vb.) temsil eden ana varlık (Entity) sınıfı.
// BaseEntity'den miras alarak Id, CreatedAt gibi temel alanlara otomatik sahip olur.
public class Asset : BaseEntity
{
    // Cihazın adı / model adı (Örn: MacBook Pro 16, Dell U2722D)
    public string Name { get; set; } = null!;
    
    // Cihazın benzersiz seri numarası. Takip için çok önemlidir.
    public string SerialNumber { get; set; } = null!;
    
    // Cihazla ilgili opsiyonel açıklamalar veya notlar
    public string? Description { get; set; }

    // Cihazın anlık durumu. Varsayılan olarak "Available" (Kullanıma hazır) atanır.
    public AssetStatus Status { get; set; } = AssetStatus.Available;

    // Cihazın kategorisi (Laptop, Monitör, Telefon vb.) — FK -> AssetCategory tablosu
    // Nullable: Eski/mevcut cihazların kategorisi olmayabilir, sonradan atanabilir.
    public Guid? CategoryId { get; set; }
    // Navigation Property: EF Core'un ilişkili kategori bilgisini (Include ile) getirmesi için kullanılır.
    public AssetCategory? Category { get; set; }

    // Navigation Property: Bu cihazın geçmişte ve şu anda kime atandığını tutan ilişki listesi (1'e Çok İlişki).
    public ICollection<Assignment> Assignments { get; set; } = new List<Assignment>();
    
    // Navigation Property: Bu cihaz için yapılan taleplerin listesi (1'e Çok İlişki).
    public ICollection<Request> Requests { get; set; } = new List<Request>();
}
