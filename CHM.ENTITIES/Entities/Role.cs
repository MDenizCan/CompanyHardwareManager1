using CHM.ENTITIES.Common;

namespace CHM.ENTITIES.Entities;

// Sistemdeki izin seviyelerini belirten Rol nesnesi ("Admin", "User", "Technician").
public class Role : BaseEntity
{
    // Rolün benzersiz adı.
    public string Name { get; set; } = null!; //seeder // Bu rol DatabaseSeeder sınıfında otomatik eklenir (seeded)
    
    // Navigation Property: Bu role sahip olan kullanıcıların ara tablo (UserRole) üzerinden tutulduğu liste.
    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
}
