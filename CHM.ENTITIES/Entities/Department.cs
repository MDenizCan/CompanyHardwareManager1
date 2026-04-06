using CHM.ENTITIES.Common;

namespace CHM.ENTITIES.Entities;

public enum DepartmentType
{
    IT = 1,
    HR = 2,
    Finance = 3,
    Sales = 4,
    Operations = 5,
    Management = 6
}

// Şirket departmanlarını temsil eden entity. (Örn: IT, Muhasebe, İK)
// Kullanıcılar bu departmanlardan birine bağlı olur.
public class Department : BaseEntity
{
    // Departmanın benzersiz adı. (Örn: "IT", "HR", "Finance")
    public DepartmentType Name { get; set; } = DepartmentType.IT;

    // Opsiyonel: Departman hakkında ek açıklama.
    public string? Description { get; set; }

    // Navigation Property: Bu departmana ait çalışanların listesi.
    public ICollection<User> Users { get; set; } = new List<User>();
}
