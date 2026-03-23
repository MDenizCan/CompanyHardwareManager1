using CHM.ENTITIES.Common;

namespace CHM.ENTITIES.Entities;

public class Assignment : BaseEntity
{
    public Guid AssetId { get; set; }
    public Asset Asset { get; set; } = null!;

    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    public DateTime AssignedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ReturnedAt { get; set; }

    // Optional: who performed the assignment (typically IT/Admin)
    public Guid? AssignedByUserId { get; set; }
    public User? AssignedByUser { get; set; }
}
