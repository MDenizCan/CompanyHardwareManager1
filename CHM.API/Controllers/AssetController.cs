using CHM.BLL.Interfaces;
using CHM.MODELS.Asset;
using CHM.MODELS.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CHM.API.Controllers;

// Dış dünyaya (Frontend/Mobil) açılan kapı (Endpoint). Cihaz (Asset) işlemleri bu kontrolcü üzerinden yapılır.
[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin,IT")] // Sadece Admin ve IT rolündeki (yetkili) kişiler bu sınıftaki işlemleri yapabilir.
public sealed class AssetController : ControllerBase
{
    private readonly IAssetService _assetService;

    public AssetController(IAssetService assetService)
    {
        _assetService = assetService;
    }

    // Sistemdeki tüm cihazları listeleyen metod.
    // Metod: GET /api/asset
    [HttpGet]
    public async Task<ActionResult<PagedResponse<AssetListResponse>>> GetAll([FromQuery] PaginationFilter filter, CancellationToken cancellationToken)
    {
        var results = await _assetService.GetAllAsync(filter, cancellationToken);
        return Ok(results);
    }

    // ID'si verilen özel bir cihazın, bütün detaylarıyla bilgisini döner. (Method: GET /api/asset/{id})
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<AssetResponse>> GetById(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var asset = await _assetService.GetByIdAsync(id, cancellationToken);
            return Ok(asset);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    // Sisteme yepyeni bir cihaz kaydeder. 201 Created döner ve yeni cihazın ID'sini Header'a yazar. (Method: POST /api/asset)
    [HttpPost]
    public async Task<ActionResult<AssetResponse>> Create([FromBody] CreateAssetRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var asset = await _assetService.CreateAsync(request, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = asset.Id }, asset);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // Mevcut bir cihazın (İsim, Durum, Seri No vb.) bilgilerini tamamen günceller. (Method: PUT /api/asset/{id})
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<AssetResponse>> Update(Guid id, [FromBody] UpdateAssetRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var asset = await _assetService.UpdateAsync(id, request, cancellationToken);
            return Ok(asset);
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

    // Cihazı sistemden (Soft Delete mantığıyla) siler. Başarılıysa içeriksiz 204 NoContent döner. (Method: DELETE /api/asset/{id})
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            await _assetService.DeleteAsync(id, cancellationToken);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }
}
