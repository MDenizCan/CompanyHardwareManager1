using CHM.ENTITIES.Common;

namespace CHM.ENTITIES.Entities;

public class Role : BaseEntity
{
    public string Name { get; set; } = null!; //seeder
    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
}
