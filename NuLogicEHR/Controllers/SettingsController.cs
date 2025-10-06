using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NuLogicEHR.Common.Exceptions;
using NuLogicEHR.DTOs;
using NuLogicEHR.Models;
using NuLogicEHR.Services;

namespace NuLogicEHR.Controllers
{
    [Route("api/v1")]
    [ApiController]
    public class SettingsController : ControllerBase
    {
        private readonly SettingService _settingService;

        public SettingsController(SettingService settingService)
        {
            _settingService = settingService;
        }

        [HttpPost("create-provider")]
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

                return StatusCode(201, new
                {
                    Data = new { ProviderId = providerId },
                    Message = "Provider created successfully",
                    StatusCode = 201
                });
            }
            catch (Exception ex)
            {
                // If it is your custom validation exception, return 400
                if (ex is CustomValidationException)
                {
                    return BadRequest(new { Message = ex.Message, StatusCode = 400 });
                }

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
        [HttpPost("create-staff")]
        public async Task<IActionResult> CreateStaff([FromBody] CreateStaffViewModel createStaffDto)
        {
            try
            {
                if (!Request.Headers.TryGetValue("TenantId", out var tenantIdHeader) ||
                    !int.TryParse(tenantIdHeader, out var tenantId))
                {
                    return BadRequest(new { Message = "TenantId header is required", StatusCode = 400 });
                }

                var result = await _settingService.CreateStaffAsync(tenantId, createStaffDto);

                return StatusCode(201, new
                {
                    Data = result,
                    Message = "Staff created successfully",
                    StatusCode = 201
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = ex.Message, StatusCode = 500 });
            }
        }

        [HttpGet("get-all-staff")]
        public async Task<IActionResult> GetAllStaff()
        {
            try
            {
                if (!Request.Headers.TryGetValue("TenantId", out var tenantIdHeader) ||
                    !int.TryParse(tenantIdHeader, out var tenantId))
                {
                    return BadRequest(new { Message = "TenantId header is required", StatusCode = 400 });
                }

                var staff = await _settingService.GetAllStaffAsync(tenantId);
                return Ok(new { Data = staff, Message = "Staff retrieved successfully", StatusCode = 200 });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = ex.Message, StatusCode = 500 });
            }
        }
    }
}