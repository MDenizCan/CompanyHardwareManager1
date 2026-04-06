using CHM.ENTITIES.Common;

namespace CHM.ENTITIES.Entities;

public enum AssetCategoryType
{
    Laptop = 1,
    Monitor = 2,
    Desktop = 3,
    Phone = 4,
    Tablet = 5,
    Keyboard = 6,
    Mouse = 7,
    Other = 8
}

// Cihazların türünü (Kategorisini) belirten entity. (Örn: Laptop, Monitör, Telefon)
// Bu sayede cihazlar filtrelenebilir ve raporlanabilir hale gelir.
public class AssetCategory : BaseEntity
{
    // Kategorinin benzersiz adı. (Örn: "Laptop", "Monitor", "Phone")
    public AssetCategoryType Name { get; set; } = AssetCategoryType.Other;

    // Opsiyonel: Kategori hakkında ek açıklama.
    public string? Description { get; set; }

    // Navigation Property: Bu kategoriye ait cihazların listesi.
    public ICollection<Asset> Assets { get; set; } = new List<Asset>();
}
