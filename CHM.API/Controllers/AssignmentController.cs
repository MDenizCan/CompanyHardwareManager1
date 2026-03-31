using System.Security.Claims;
using CHM.BLL.Interfaces;
using CHM.MODELS.Assignment;
using CHM.MODELS.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CHM.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize] // Bütün işlemler için giriş yapılmış olması zorunlu
public sealed class AssignmentController : ControllerBase
{
    private readonly IAssignmentService _assignments;

    public AssignmentController(IAssignmentService assignments)
    {
        _assignments = assignments;
    }

    [HttpPost("assign")]
    [Authorize(Roles = "Admin,IT")] // Sadece yetkililer cihaz zimmetleyebilir
    public async Task<ActionResult<AssignmentResponseDto>> AssignAsset([FromBody] AssignmentRequestDto request, CancellationToken cancellationToken)
    {
        var assignedByUserIdRaw = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
        if (!Guid.TryParse(assignedByUserIdRaw, out var assignedByUserId))
            return Unauthorized(new { message = "Geçersiz oturum." });

        try
        {
            var result = await _assignments.AssignAssetAsync(request, assignedByUserId, cancellationToken);
            return Ok(result);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("return")]
    [Authorize(Roles = "Admin,IT")] // Sadece yetkililer cihaz iadesi alabilir
    public async Task<ActionResult<AssignmentResponseDto>> ReturnAsset([FromBody] ReturnAssetDto request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _assignments.ReturnAssetAsync(request, cancellationToken);
            return Ok(result);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("user/{userId:guid}")]
    public async Task<ActionResult<PagedResponse<AssignmentResponseDto>>> GetUserAssignments(Guid userId, [FromQuery] PaginationFilter filter, CancellationToken cancellationToken)
    {
        // Kullanıcı kendi cihazlarını görebilir, Admin/IT herkesin cihazlarını görebilir.
        var requestUserIdRaw = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
        Guid.TryParse(requestUserIdRaw, out var currentUserId);
        var isAdmin = User.IsInRole("Admin") || User.IsInRole("IT");

        if (!isAdmin && currentUserId != userId)
        {
            return Forbid(); // Başkasının cihazlarına bakamaz (403)
        }

        var results = await _assignments.GetUserAssignmentsAsync(userId, filter, cancellationToken);
        return Ok(results);
    }

    [HttpGet("active")]
    [Authorize(Roles = "Admin,IT")]
    public async Task<ActionResult<PagedResponse<AssignmentResponseDto>>> GetAllActive([FromQuery] PaginationFilter filter, CancellationToken cancellationToken)
    {
        var results = await _assignments.GetAllActiveAssignmentsAsync(filter, cancellationToken);
        return Ok(results);
    }
}
