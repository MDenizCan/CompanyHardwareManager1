using CHM.BLL.Interfaces;
using CHM.MODELS.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CHM.API.Controllers;

// Kullanıcıların sisteme giriş yaptığı, kaydolduğu ve token(anahtar) aldığı kimlik doğrulama uç noktası.
[ApiController]
[Route("api/[controller]")]
public sealed class AuthController : ControllerBase
{
    private readonly IAuthService _auth;

    public AuthController(IAuthService auth)
    {
        _auth = auth;
    }

    // Yeni kullanıcıların sisteme kayıt olduğu API ucu. 
    // [AllowAnonymous] ile bu metoda herkesin (Token olmadan) erişebilmesini sağlar. (Method: POST /api/auth/register)
    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request, CancellationToken cancellationToken)
    {
        try
        {
            await _auth.RegisterAsync(request, cancellationToken);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // E-posta veya kullanıcı adı ile giriş yapılarak JWT (Access ve Refresh Token) almanızı sağlayan API ucu. (Method: POST /api/auth/login)
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<TokenResponse>> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var tokens = await _auth.LoginAsync(request, cancellationToken);
            return Ok(tokens);
        }
        catch (InvalidOperationException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
    }

    // Mevcut Access Token'ın süresi (15 dk) bittiğinde kullanıcıdan tekrar şifre istememek için 
    // eldeki uzun süreli (7 Gün) Refresh Token'ı verip her iki token'ı da yeniler. (Method: POST /api/auth/refresh)
    [HttpPost("refresh")]
    [AllowAnonymous]
    public async Task<ActionResult<TokenResponse>> Refresh([FromBody] RefreshTokenRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var tokens = await _auth.RefreshAsync(request, cancellationToken);
            return Ok(tokens);
        }
        catch (InvalidOperationException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
    }

    // Sisteme giriş yapmış mevcut kullanıcının şifresini değiştirmek için kullanılır.
    // [Authorize] sadece Login olmuş (elinde Token olan) kişilerin girmesini zorunlu kılar. (Method: POST /api/auth/change-password)
    [HttpPost("change-password")]
    [Authorize]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request, CancellationToken cancellationToken)
    {
        var userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
        if (!Guid.TryParse(userIdValue, out var userId))
            return Unauthorized(new { message = "Invalid token." });

        try
        {
            await _auth.ChangePasswordAsync(userId, request, cancellationToken);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}

