using Microsoft.AspNetCore.Mvc;
using NuLogicEHR.Services;
using NuLogicEHR.ViewModels;

namespace NuLogicEHR.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppointmentController : ControllerBase
    {
        private readonly AppointmentService _appointmentService;
        private readonly ILogger<AppointmentController> _logger;

        public AppointmentController(AppointmentService appointmentService, ILogger<AppointmentController> logger)
        {
            _appointmentService = appointmentService;
            _logger = logger;
        }

        private bool TryGetTenantId(out int tenantId)
        {
            tenantId = 0;
            return Request.Headers.TryGetValue("TenantId", out var tenantIdHeader) && 
                   int.TryParse(tenantIdHeader, out tenantId);
        }

        [HttpPost("Patient-Create-Appointments")]
        public async Task<IActionResult> CreateAppointment([FromBody] AppointmentCreateViewModel dto)
        {
            if (!TryGetTenantId(out var tenantId))
                return BadRequest(new { Message = "TenantId header is required", StatusCode = 400 });

            try
            {
                var appointmentId = await _appointmentService.CreateAppointmentAsync(tenantId, dto);
                return StatusCode(201, new
                {
                    Data = new { AppointmentId = appointmentId },
                    Message = "Appointment created successfully",
                    StatusCode = 201
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating appointment");
                return StatusCode(500, new { Message = "Internal server error", StatusCode = 500 });
            }
        }

        [HttpGet("Get-All-Patient-Appointments")]
        public async Task<IActionResult> GetAppointments()
        {
            if (!TryGetTenantId(out var tenantId))
                return BadRequest(new { Message = "TenantId header is required", StatusCode = 400 });

            try
            {
                var appointments = await _appointmentService.GetAppointmentsAsync(tenantId);
                return Ok(new
                {
                    Data = appointments,
                    Message = "Appointments retrieved successfully",
                    StatusCode = 200
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving appointments");
                return StatusCode(500, new { Message = "Internal server error", StatusCode = 500 });
            }
        }
    }
}
