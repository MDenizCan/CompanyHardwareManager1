using System.Security.Cryptography;
using CHM.ENTITIES.Entities;
using Microsoft.EntityFrameworkCore;

namespace CHM.INFRASTRUCTURE.Seed;

// Uygulama ilk başladığında veritabanında "olmazsa olmaz" olan temel verileri (Roller gibi)
// otomatik olarak ekleyen (seed eden) sınıf.
public static class DatabaseSeeder
{
    // Sistemde her zaman bulunması gereken 3 temel rol.
    private static readonly RoleType[] DefaultRoles = [RoleType.Admin, RoleType.IT, RoleType.Employee];

    // Sistemde her zaman bulunması gereken varsayılan cihaz kategorileri.
    private static readonly AssetCategoryType[] DefaultCategories = [AssetCategoryType.Laptop, AssetCategoryType.Monitor, AssetCategoryType.Desktop, AssetCategoryType.Phone, AssetCategoryType.Tablet, AssetCategoryType.Keyboard, AssetCategoryType.Mouse, AssetCategoryType.Other];

    // Sistemde her zaman bulunması gereken varsayılan departmanlar.
    private static readonly DepartmentType[] DefaultDepartments = [DepartmentType.IT, DepartmentType.HR, DepartmentType.Finance, DepartmentType.Sales, DepartmentType.Operations, DepartmentType.Management];

    public static async Task SeedAsync(AppDbContext db, CancellationToken cancellationToken = default)
    {
        try 
        {
            Console.WriteLine("--> SeedAsync started...");
            
            // Her bir rol için veritabanında olup olmadığını kontrol edeceğiz.
            foreach (var roleName in DefaultRoles)
            {
                var roleNameStr = roleName.ToString();
                // EF Core enum string comparison workaround
                var exists = await db.Roles.IgnoreQueryFilters().AnyAsync(r => r.Name == roleName, cancellationToken);
                if (!exists)
                {
                    Console.WriteLine($"--> Creating role: {roleName}");
                    db.Roles.Add(new Role { Id = Guid.NewGuid(), Name = roleName, CreatedAt = DateTime.UtcNow, IsDeleted = false });
                }
            }

            // Değişiklikleri kaydet (Roller oluşsun ki kullanıcı eklerken bulabilelim)
            await db.SaveChangesAsync(cancellationToken);

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

            // Kategorileri ve departmanları kaydet.
            await db.SaveChangesAsync(cancellationToken);

            // Varsayılan Admin Kullanıcısı Kontrolü
            // Use ToList to avoid EF Core SQL conversion issues with enums if needed
            var allRoles = await db.Roles.IgnoreQueryFilters().ToListAsync(cancellationToken);
            var adminRole = allRoles.FirstOrDefault(r => r.Name == RoleType.Admin);
            
            if (adminRole != null)
            {
                var adminUserExists = await db.Users.IgnoreQueryFilters().AnyAsync(u => u.Username == "admin", cancellationToken);
                if (!adminUserExists)
                {
                    Console.WriteLine("--> Creating default admin user...");
                    var adminUser = new User
                    {
                        Id = Guid.NewGuid(),
                        Username = "admin",
                        Email = "admin@company.com",
                        PasswordHash = HashPassword("Admin123!"),
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow,
                        IsDeleted = false
                    };

                    db.Users.Add(adminUser);
                    
                    db.UserRoles.Add(new UserRole
                    {
                        UserId = adminUser.Id,
                        RoleId = adminRole.Id
                    });
                }
            }

            // Yapılan "Add" işlemlerini kalıcı olarak veritabanına kaydet.
            await db.SaveChangesAsync(cancellationToken);
            Console.WriteLine("--> SeedAsync completed.");
        } 
        catch (Exception ex)
        {
            Console.WriteLine($"--> SEED ERROR: {ex.Message}");
            if (ex.InnerException != null) Console.WriteLine($"--> INNER: {ex.InnerException.Message}");
            throw;
        }
    }

    // Hash method copied from PasswordHasher to avoid circular dependency
    private static string HashPassword(string password)
    {
        var salt = RandomNumberGenerator.GetBytes(16);
        var key = Rfc2898DeriveBytes.Pbkdf2(
            password,
            salt,
            100_000,
            HashAlgorithmName.SHA256,
            32);

        return $"PBKDF2$100000${Convert.ToBase64String(salt)}${Convert.ToBase64String(key)}";
    }
}
