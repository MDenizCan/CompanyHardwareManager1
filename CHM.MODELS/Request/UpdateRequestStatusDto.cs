namespace CHM.MODELS.Request;

public sealed class UpdateRequestStatusDto
{
    // 0=Pending, 1=Approved, 2=Rejected, 3=Completed
    public int Status { get; set; }
}
