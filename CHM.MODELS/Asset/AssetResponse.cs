namespace CHM.MODELS.Asset;

// API'den tek bir Asset detay verisi (GetByIdAsync, CreateAsync vb.) istendiğinde dönen detaylı DTO.
// Listelemedeki gibi özet veri değil, daha tam (Açıklama, Tarih vb.) veri içerir ama yine de Entity'yi doğrudan dışarı açmaz.
public sealed class AssetResponse
{
    // Cihazın benzersiz kimliği
    public Guid Id { get; set; }
    
    // Cihazın adı (Örn: Dell Laptop)
    public string Name { get; set; } = null!;
    
    // Cihazın seri numarası
    public string SerialNumber { get; set; } = null!;
    
    // Opsiyonel: Cihazın detaylı açıklaması
    public string? Description { get; set; }
    
    // Cihazın durumu (string olarak, Örn: "Available", "InUse")
    public string Status { get; set; } = null!;
    
    // Cihazın kategorisi (varsa)
    public Guid? CategoryId { get; set; }
    public string? CategoryName { get; set; }
    
    // Cihazın ne zaman oluşturulduğu
    public DateTime CreatedAt { get; set; }
    
    // Opsiyonel: Cihaz ne zaman güncellendi
    public DateTime? UpdatedAt { get; set; }
}
