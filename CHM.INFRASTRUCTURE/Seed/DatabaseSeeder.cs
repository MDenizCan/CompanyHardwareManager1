using CHM.ENTITIES.Entities;
using Microsoft.EntityFrameworkCore;

namespace CHM.INFRASTRUCTURE.Seed;

// Uygulama ilk başladığında veritabanında "olmazsa olmaz" olan temel verileri (Roller gibi)
// otomatik olarak ekleyen (seed eden) sınıf.
public static class DatabaseSeeder
{
    // Sistemde her zaman bulunması gereken 3 temel rol.
    private static readonly string[] DefaultRoles = ["Admin", "IT", "Employee"];

    public static async Task SeedAsync(AppDbContext db, CancellationToken cancellationToken = default)
    {
        // Her bir rol için veritabanında olup olmadığını kontrol edeceğiz.
        foreach (var roleName in DefaultRoles)
        {
            // IgnoreQueryFilters() KULLANIMI ÇOK ÖNEMLİ:
            // Eğer bunu kullanmazsak ve rol daha önce silinmişse (IsDeleted = true),
            // AnyAsync onu görmez ve yeni bir tane eklemeye çalışarak veritabanında
            // Unique Index (benzersiz isim) hatasına sebep olur.
            var exists = await db.Roles.IgnoreQueryFilters().AnyAsync(r => r.Name == roleName, cancellationToken);
            
            // Eğer rol veritabanında HİÇ yoksa (ne silinmiş ne de aktif olarak), o zaman yeni oluştur ve ekle.
            if (!exists)
            {
                db.Roles.Add(new Role
                {
                    Id = Guid.NewGuid(),
                    Name = roleName,
                    CreatedAt = DateTime.UtcNow,
                    IsDeleted = false
                });
            }
        }

        // Yapılan "Add" işlemlerini kalıcı olarak veritabanına kaydet.
        await db.SaveChangesAsync(cancellationToken);
    }
}
