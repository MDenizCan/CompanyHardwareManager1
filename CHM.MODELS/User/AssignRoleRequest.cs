namespace CHM.MODELS.User;

// Rol atama işleminde istenecek olan model
public sealed class AssignRoleRequest
{
    public Guid UserId { get; set; }
    public string RoleName { get; set; } = string.Empty;
}
