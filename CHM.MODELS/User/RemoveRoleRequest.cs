namespace CHM.MODELS.User;

// Rol silme işleminde istenecek olan model
public sealed class RemoveRoleRequest
{
    public Guid UserId { get; set; }
    public string RoleName { get; set; } = string.Empty;
}
