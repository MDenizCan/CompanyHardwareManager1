using CHM.ENTITIES.Common;

namespace CHM.ENTITIES.Entities;

// Şirket departmanlarını temsil eden entity. (Örn: IT, Muhasebe, İK)
// Kullanıcılar bu departmanlardan birine bağlı olur.
public class Department : BaseEntity
{
    // Departmanın benzersiz adı. (Örn: "IT", "HR", "Finance")
    public string Name { get; set; } = null!;

    // Opsiyonel: Departman hakkında ek açıklama.
    public string? Description { get; set; }

    // Navigation Property: Bu departmana ait çalışanların listesi.
    public ICollection<User> Users { get; set; } = new List<User>();
}
