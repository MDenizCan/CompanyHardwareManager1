namespace CHM.MODELS.Asset;

public sealed class AssetResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string SerialNumber { get; set; } = null!;
    public string? Description { get; set; }
    public string Status { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
