using CHM.ENTITIES.Common;

namespace CHM.ENTITIES.Entities;

public class User : BaseEntity
{
    public string Username { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;

    public bool IsActive { get; set; } = true;

    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    public ICollection<Assignment> Assignments { get; set; } = new List<Assignment>();
    public ICollection<Request> Requests { get; set; } = new List<Request>();
    public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
}
