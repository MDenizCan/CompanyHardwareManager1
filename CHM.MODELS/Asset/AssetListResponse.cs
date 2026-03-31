namespace CHM.MODELS.Asset;

// API'den liste halinde Asset verisi (GetAllAsync vb.) istendiğinde dönen DTO (Data Transfer Object).
// Sadece listelemede gösterilmesi gereken temel bilgileri içerir. Veritabanı Entity'sini dış dünyaya kapatmak (Gizlilik ve Güvenlik) için kullanılır.
public sealed class AssetListResponse
{
    // Cihazın benzersiz numarası
    public Guid Id { get; set; }
    
    // Cihaz adı
    public string Name { get; set; } = null!;
    
    // Seri numarası
    public string SerialNumber { get; set; } = null!;
    
    // Cihazın durumu.
    public string Status { get; set; } = null!;

    // Cihazın kategorisi (varsa)
    public string? CategoryName { get; set; }
}
