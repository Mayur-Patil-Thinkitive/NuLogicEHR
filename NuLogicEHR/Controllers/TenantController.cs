using Microsoft.AspNetCore.Mvc;
using NuLogicEHR.Services;
using NuLogicEHR.ViewModels;

namespace NuLogicEHR.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TenantController : ControllerBase
    {
        private readonly TenantService _tenantService;

        public TenantController(TenantService tenantService)
        {
            _tenantService = tenantService;
        }

        [HttpPost("Create-Tenant")]
        public async Task<IActionResult> CreateTenant([FromBody] TenantCreateViewModel dto)
        {
            try
            {
                if (string.IsNullOrEmpty(dto.HospitalName))
                    return BadRequest(new { Message = "Hospital name is required", StatusCode = 400 });

                var tenant = await _tenantService.CreateTenantAsync(dto.HospitalName);

                var response = new TenantResponseViewModel
                {
                    Id = tenant.Id,
                    HospitalName = tenant.HospitalName,
                    CreatedBy = tenant.CreatedBy
                };

                return StatusCode(201, new { Data = response, Message = "Tenant created successfully", StatusCode = 201 });
            }
            catch
            {
                return StatusCode(500, new { Message = "Internal server error", StatusCode = 500 });
            }
        }
    }
}
