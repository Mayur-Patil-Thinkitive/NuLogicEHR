using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NuLogicEHR.Services;
using NuLogicEHR.Models;

namespace NuLogicEHR.Controllers
{
    [Route("api/v1/settings")]
    [ApiController]
    public class Settings : ControllerBase
    {
        private readonly SettingService _settingService;

        public Settings(SettingService settingService)
        {
            _settingService = settingService;
        }

        [HttpPost("provider")]
        public async Task<IActionResult> CreateProvider([FromBody] Provider provider)
        {
            try
            {
                if (!Request.Headers.TryGetValue("TenantId", out var tenantIdHeader) ||
                    !int.TryParse(tenantIdHeader, out var tenantId))
                {
                    return BadRequest(new { Message = "TenantId header is required", StatusCode = 400 });
                }

                var providerId = await _settingService.CreateProviderAsync(tenantId, provider);
                return StatusCode(201, new { Data = new { ProviderId = providerId }, Message = "Provider created successfully", StatusCode = 201 });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = ex.Message, StatusCode = 500 });
            }
        }

        [HttpGet("get-all-providers")]
        public async Task<IActionResult> GetProviders()
        {
            try
            {
                if (!Request.Headers.TryGetValue("TenantId", out var tenantIdHeader) ||
                    !int.TryParse(tenantIdHeader, out var tenantId))
                {
                    return BadRequest(new { Message = "TenantId header is required", StatusCode = 400 });
                }

                var providers = await _settingService.GetAllProvidersAsync(tenantId);
                return Ok(new { Data = providers, Message = "Providers retrieved successfully", StatusCode = 200 });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = ex.Message, StatusCode = 500 });
            }
        }
    }
}
