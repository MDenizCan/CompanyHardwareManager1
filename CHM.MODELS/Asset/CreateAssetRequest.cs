namespace CHM.MODELS.Asset;

public sealed class CreateAssetRequest
{
    public string Name { get; set; } = null!;
    public string SerialNumber { get; set; } = null!;
    public string? Description { get; set; }
}
