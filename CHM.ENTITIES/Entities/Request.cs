using CHM.ENTITIES.Common;

namespace CHM.ENTITIES.Entities;

public enum RequestType
{
    FaultReport,
    ReturnRequest
}

public enum RequestStatus
{
    Pending,
    Approved,
    Rejected,
    Completed
}
public class Request : BaseEntity
{
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    public Guid? AssetId { get; set; }
    public Asset? Asset { get; set; }

    public RequestType Type { get; set; }

    public string Description { get; set; } = null!;

    public RequestStatus Status { get; set; } = RequestStatus.Pending;
}
