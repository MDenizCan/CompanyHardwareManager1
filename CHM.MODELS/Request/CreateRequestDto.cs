namespace CHM.MODELS.Request;

public sealed class CreateRequestDto
{
    // 0=FaultReport, 1=ReturnRequest
    public int Type { get; set; }
    
    // Her iki talep türü de (Arıza, İade) bir cihaza ait olmak zorundadır.
    public Guid AssetId { get; set; } 
    public string Description { get; set; } = string.Empty;
}
