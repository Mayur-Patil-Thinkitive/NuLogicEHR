using Microsoft.AspNetCore.Mvc;
using NuLogicEHR.Services;
using NuLogicEHR.ViewModels;

namespace NuLogicEHR.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PatientController : ControllerBase
    {
        private readonly PatientService _patientService;

        public PatientController(PatientService patientService)
        {
            _patientService = patientService;
        }

        [HttpPost]
        public async Task<IActionResult> CreatePatient([FromBody] PatientCreateViewModel dto)
        {
            try
            {
                if (!Request.Headers.TryGetValue("TenantId", out var tenantIdHeader) || 
                    !int.TryParse(tenantIdHeader, out var tenantId))
                {
                    return BadRequest(new { Message = "TenantId header is required", StatusCode = 400 });
                }

                var response = await _patientService.CreatePatientAsync(tenantId, dto);
                return StatusCode(201, new { Data = response, Message = "Patient created successfully", StatusCode = 201 });
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { Message = ex.Message, StatusCode = 404 });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = $"Internal server error: {ex.Message}", StatusCode = 500 });
            }
        }

        [HttpGet("{tenantId}")]
        public async Task<IActionResult> GetPatients(int tenantId)
        {
            try
            {
                var patients = await _patientService.GetPatientsAsync(tenantId);
                var message = patients.Any() ? "Patients retrieved successfully" : "No patients found for this tenant";
                return Ok(new { Data = patients, Message = message, StatusCode = 200 });
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { Message = ex.Message, StatusCode = 404 });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = $"Internal server error: {ex.Message}", StatusCode = 500 });
            }
        }
    }
}
