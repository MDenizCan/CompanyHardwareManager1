namespace CHM.MODELS.Assignment;

public sealed class ReturnAssetDto
{
    public Guid AssignmentId { get; set; }
    public int ReturnStatus { get; set; } // Orijinal enum değerleriyle eşleşecek: 0=Available, 2=Faulty vb.
    public string? ReturnNote { get; set; }
}
