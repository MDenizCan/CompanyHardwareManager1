namespace CHM.MODELS.Asset;

// API üzerinden mevcut bir cihazı güncellemek (PUT /api/Assets/{id}) isteyen istemcinin göndereceği Request Modeli.
// CreateAssetRequest'ten farklı olarak "Status" (Durum) verisini de içerir, böylece güncelleme yaparken makinenin arızaya veya boşa düştüğünü seçebiliriz.
public sealed class UpdateAssetRequest
{
    // Güncellenecek cihazın yeni adı / model adı.
    public string Name { get; set; } = null!;
    
    // Güncellenecek cihazın seri numarası.
    public string SerialNumber { get; set; } = null!;
    
    // Cihazın yeni açıklaması.
    public string? Description { get; set; }
    
    // Cihazın yeni durumu, Enum karşılığı int olarak istenir. Örneğin Arızalı(Faulty) demek için 2 göndermesi gerekir.
    public int Status { get; set; }

    // Cihazın güncellenen kategorisi. Null gönderilirse kategori değişmez, kategorisiz kalır.
    public Guid? CategoryId { get; set; }
}
