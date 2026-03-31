using CHM.BLL.Interfaces;
using CHM.MODELS.User;
using CHM.MODELS.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CHM.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")] // Kullanıcı listesi ve rol atama — sadece Admin yapabilir
public sealed class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    // Sistemdeki tüm kullanıcıları ve sahip oldukları rolleri listeler. (Method: GET /api/user)
    [HttpGet]
    public async Task<ActionResult<PagedResponse<UserResponse>>> GetAll([FromQuery] PaginationFilter filter, CancellationToken cancellationToken)
    {
        var results = await _userService.GetAllAsync(filter, cancellationToken);
        return Ok(results);
    }

    // Seçilen kullanıcıya, yazılan rolü (Admin, IT) atar. (Method: POST /api/user/assign-role)
    [HttpPost("assign-role")]
    public async Task<IActionResult> AssignRole([FromBody] AssignRoleRequest request, CancellationToken cancellationToken)
    {
        try
        {
            await _userService.AssignRoleAsync(request, cancellationToken);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }
}
