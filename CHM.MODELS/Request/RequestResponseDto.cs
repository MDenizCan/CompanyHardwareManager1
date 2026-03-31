namespace CHM.MODELS.Request;

public sealed class RequestResponseDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    
    public Guid? AssetId { get; set; }
    public string? AssetName { get; set; }
    public string? AssetSerialNumber { get; set; }
    
    public string Type { get; set; } = string.Empty;   // "FaultReport", "ReturnRequest" vb.
    public string Status { get; set; } = string.Empty; // "Pending", "Approved" vb.
    
    public string Description { get; set; } = string.Empty;
    
    public DateTime CreatedAt { get; set; }
}
