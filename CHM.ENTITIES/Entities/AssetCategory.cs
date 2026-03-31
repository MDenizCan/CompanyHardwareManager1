using CHM.ENTITIES.Common;

namespace CHM.ENTITIES.Entities;

// Cihazların türünü (Kategorisini) belirten entity. (Örn: Laptop, Monitör, Telefon)
// Bu sayede cihazlar filtrelenebilir ve raporlanabilir hale gelir.
public class AssetCategory : BaseEntity
{
    // Kategorinin benzersiz adı. (Örn: "Laptop", "Monitor", "Phone")
    public string Name { get; set; } = null!;

    // Opsiyonel: Kategori hakkında ek açıklama.
    public string? Description { get; set; }

    // Navigation Property: Bu kategoriye ait cihazların listesi.
    public ICollection<Asset> Assets { get; set; } = new List<Asset>();
}
