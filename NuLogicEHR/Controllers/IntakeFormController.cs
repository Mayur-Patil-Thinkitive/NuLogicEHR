using Microsoft.AspNetCore.Mvc;
using NuLogicEHR.Services;
using NuLogicEHR.ViewModels;
using NuLogicEHR.Models;

namespace NuLogicEHR.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IntakeFormController : ControllerBase
    {
        private readonly IntakeFormService _service;
        private readonly ILogger<IntakeFormController> _logger;

        public IntakeFormController(IntakeFormService service, ILogger<IntakeFormController> logger)
        {
            _service = service;
            _logger = logger;
        }

        private bool TryGetTenantId(out int tenantId)
        {
            tenantId = 0;
            return Request.Headers.TryGetValue("TenantId", out var tenantIdHeader) && 
                   int.TryParse(tenantIdHeader, out tenantId);
        }

        [HttpPost("History")]
        public async Task<IActionResult> CreateHistory([FromBody] PatientIntakeHistoryViewModel dto)
        {
            if (!TryGetTenantId(out var tenantId))
                return BadRequest(new { Message = "TenantId header is required", StatusCode = 400 });

            try
            {
                var id = await _service.CreateHistoryAsync(tenantId, dto);
                return StatusCode(201, new { Data = new { Id = id }, Message = "Intake History created", StatusCode = 201 });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating history");
                return StatusCode(500, new { Message = "Internal server error", StatusCode = 500 });
            }
        }

        [HttpGet("GetByID-History/{id}")]
        public async Task<IActionResult> GetHistory(int id)
        {
            if (!TryGetTenantId(out var tenantId))
                return BadRequest(new { Message = "TenantId header is required", StatusCode = 400 });

            try
            {
                var history = await _service.GetHistoryByIdAsync(tenantId, id);
                if (history == null)
                    return NotFound(new { Message = "History not found", StatusCode = 404 });

                return Ok(new { Data = history, StatusCode = 200 });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving history");
                return StatusCode(500, new { Message = "Internal server error", StatusCode = 500 });
            }
        }

        [HttpPut("Update-History/{id}")]
        public async Task<IActionResult> UpdateHistory(int id, [FromBody] PatientIntakeHistoryViewModel dto)
        {
            if (!TryGetTenantId(out var tenantId))
                return BadRequest(new { Message = "TenantId header is required", StatusCode = 400 });

            try
            {
                await _service.UpdateHistoryAsync(tenantId, id, dto);
                return Ok(new { Message = "History updated", StatusCode = 200 });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating history");
                return StatusCode(500, new { Message = "Internal server error", StatusCode = 500 });
            }
        }

        [HttpDelete("Delete-History/{id}")]
        public async Task<IActionResult> DeleteHistory(int id)
        {
            if (!TryGetTenantId(out var tenantId))
                return BadRequest(new { Message = "TenantId header is required", StatusCode = 400 });

            try
            {
                await _service.DeleteHistoryAsync(tenantId, id);
                return Ok(new { Message = "History deleted", StatusCode = 200 });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting history");
                return StatusCode(500, new { Message = "Internal server error", StatusCode = 500 });
            }
        }
    }
}
