namespace CHM.MODELS.Asset;

public sealed class AssetListResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string SerialNumber { get; set; } = null!;
    public string Status { get; set; } = null!;
}
