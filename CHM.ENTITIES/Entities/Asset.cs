using CHM.ENTITIES.Common;

namespace CHM.ENTITIES.Entities;

public enum AssetStatus
{
    Available,
    InUse,
    Faulty,
    UnderMaintenance,
    Retired
}

public class Asset : BaseEntity
{
    public string Name { get; set; } = null!;
    public string SerialNumber { get; set; } = null!;
    public string? Description { get; set; }

    public AssetStatus Status { get; set; } = AssetStatus.Available;

    public ICollection<Assignment> Assignments { get; set; } = new List<Assignment>();
    public ICollection<Request> Requests { get; set; } = new List<Request>();
}
