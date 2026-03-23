namespace CHM.MODELS.Asset;

public sealed class UpdateAssetRequest
{
    public string Name { get; set; } = null!;
    public string SerialNumber { get; set; } = null!;
    public string? Description { get; set; }
    public int Status { get; set; }
}
