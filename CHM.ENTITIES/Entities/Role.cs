using CHM.ENTITIES.Common;

namespace CHM.ENTITIES.Entities;

public enum RoleType
{
    Admin = 1,
    IT = 2,
    Employee = 3
}

// Sistemdeki izin seviyelerini belirten Rol nesnesi ("Admin", "User", "Technician").
public class Role : BaseEntity
{
    // Rolün benzersiz adı.
    public RoleType Name { get; set; } = RoleType.Employee; //seeder // Bu rol DatabaseSeeder sınıfında otomatik eklenir (seeded)
    
    // Navigation Property: Bu role sahip olan kullanıcıların ara tablo (UserRole) üzerinden tutulduğu liste.
    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
}
