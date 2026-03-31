namespace CHM.MODELS.Asset;

// API üzerinden yeni bir cihaz kaydetmek (POST /api/Assets) isteyen istemcinin göndereceği Request Modeli.
// Id, CreatedAt veya Status gibi değerler buraya dahil edilmemiştir, çünkü bu verileri istemcinin (Frontend vb.) kendisi belirleyemez (Serviste otomatik atanır).
public sealed class CreateAssetRequest
{
    // Eklenecek cihazın adı / model adı.
    public string Name { get; set; } = null!;
    
    // Eklenecek cihazın seri numarası (Eşsiz olmalıdır).
    public string SerialNumber { get; set; } = null!;
    
    // Eklenecekcihazla ilgili ek not.
    public string? Description { get; set; }

    // Cihazın kategorisi (Laptop, Monitor vb.) — AssetCategory ID'si gönderilmeli.
    // Zorunlu değildir ama girilmesi şiddetle tavsiye edilir.
    public Guid? CategoryId { get; set; }
}
