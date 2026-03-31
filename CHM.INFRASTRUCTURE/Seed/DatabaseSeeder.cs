using CHM.ENTITIES.Entities;
using Microsoft.EntityFrameworkCore;

namespace CHM.INFRASTRUCTURE.Seed;

// Uygulama ilk başladığında veritabanında "olmazsa olmaz" olan temel verileri (Roller gibi)
// otomatik olarak ekleyen (seed eden) sınıf.
public static class DatabaseSeeder
{
    // Sistemde her zaman bulunması gereken 3 temel rol.
    private static readonly string[] DefaultRoles = ["Admin", "IT", "Employee"];

    // Sistemde her zaman bulunması gereken varsayılan cihaz kategorileri.
    private static readonly string[] DefaultCategories = ["Laptop", "Monitor", "Desktop", "Phone", "Tablet", "Keyboard", "Mouse", "Other"];

    // Sistemde her zaman bulunması gereken varsayılan departmanlar.
    private static readonly string[] DefaultDepartments = ["IT", "HR", "Finance", "Sales", "Operations", "Management"];

    public static async Task SeedAsync(AppDbContext db, CancellationToken cancellationToken = default)
    {
        // Her bir rol için veritabanında olup olmadığını kontrol edeceğiz.
        foreach (var roleName in DefaultRoles)
        {
            var exists = await db.Roles.IgnoreQueryFilters().AnyAsync(r => r.Name == roleName, cancellationToken);
            if (!exists)
            {
                db.Roles.Add(new Role { Id = Guid.NewGuid(), Name = roleName, CreatedAt = DateTime.UtcNow, IsDeleted = false });
            }
        }

        // Her bir cihaz kategorisi için kontrol.
        foreach (var categoryName in DefaultCategories)
        {
            var exists = await db.AssetCategories.IgnoreQueryFilters().AnyAsync(c => c.Name == categoryName, cancellationToken);
            if (!exists)
            {
                db.AssetCategories.Add(new AssetCategory { Id = Guid.NewGuid(), Name = categoryName, CreatedAt = DateTime.UtcNow, IsDeleted = false });
            }
        }

        // Her bir departman için kontrol.
        foreach (var departmentName in DefaultDepartments)
        {
            var exists = await db.Departments.IgnoreQueryFilters().AnyAsync(d => d.Name == departmentName, cancellationToken);
            if (!exists)
            {
                db.Departments.Add(new Department { Id = Guid.NewGuid(), Name = departmentName, CreatedAt = DateTime.UtcNow, IsDeleted = false });
            }
        }

        // Yapılan "Add" işlemlerini kalıcı olarak veritabanına kaydet.
        await db.SaveChangesAsync(cancellationToken);
    }
}
