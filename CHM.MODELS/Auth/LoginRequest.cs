namespace CHM.MODELS.Auth;

public sealed class LoginRequest
{
    public string UsernameOrEmail { get; set; } = null!;
    public string Password { get; set; } = null!;
}

