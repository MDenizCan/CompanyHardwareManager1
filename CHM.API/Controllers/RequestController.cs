using System.Security.Claims;
using CHM.BLL.Interfaces;
using CHM.MODELS.Request;
using CHM.MODELS.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CHM.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize] // Bütün işlemler için giriş yapılmış olması zorunlu
public sealed class RequestController : ControllerBase
{
    private readonly IRequestService _requests;

    public RequestController(IRequestService requests)
    {
        _requests = requests;
    }

    // Kullanıcının yeni bir talep oluşturması (Arıza, Yeni İhtiyaç vb.)
    [HttpPost]
    public async Task<ActionResult<RequestResponseDto>> Create([FromBody] CreateRequestDto request, CancellationToken cancellationToken)
    {
        var userIdRaw = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
        if (!Guid.TryParse(userIdRaw, out var userId))
            return Unauthorized(new { message = "Geçersiz oturum." });

        try
        {
            var result = await _requests.CreateRequestAsync(userId, request, cancellationToken);
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

    // Kullanıcının kendi açtığı talepleri görmesi
    [HttpGet("my-requests")]
    public async Task<ActionResult<PagedResponse<RequestResponseDto>>> GetMyRequests([FromQuery] PaginationFilter filter, CancellationToken cancellationToken)
    {
        var userIdRaw = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
        if (!Guid.TryParse(userIdRaw, out var userId))
            return Unauthorized(new { message = "Geçersiz oturum." });

        var results = await _requests.GetUserRequestsAsync(userId, filter, cancellationToken);
        return Ok(results);
    }

    // Tüm personel taleplerinin listelenmesi (Sadece yetkililer görebilir)
    [HttpGet("all")]
    [Authorize(Roles = "Admin,IT")]
    public async Task<ActionResult<PagedResponse<RequestResponseDto>>> GetAll([FromQuery] PaginationFilter filter, CancellationToken cancellationToken)
    {
        var results = await _requests.GetAllAsync(filter, cancellationToken);
        return Ok(results);
    }

    // Talebin durumunu güncelleme (Onaylama, Reddetme, Çözüldü olarak işaretleme)
    [HttpPut("{id:guid}/status")]
    [Authorize(Roles = "Admin,IT")]
    public async Task<ActionResult<RequestResponseDto>> UpdateStatus(Guid id, [FromBody] UpdateRequestStatusDto request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _requests.UpdateStatusAsync(id, request, cancellationToken);
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
}
