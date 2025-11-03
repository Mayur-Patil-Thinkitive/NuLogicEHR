using Microsoft.AspNetCore.Mvc;
using NuLogicEHR.Services;
using NuLogicEHR.ViewModels;
using NuLogicEHR.Enums;

namespace NuLogicEHR.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PatientController : ControllerBase
    {
        private readonly PatientService _patientService;
        private readonly ILogger<PatientController> _logger;

        public PatientController(PatientService patientService, ILogger<PatientController> logger)
        {
            _patientService = patientService;
            _logger = logger;
        }

        private bool TryGetTenantId(out int tenantId)
        {
            tenantId = 0;

            if (!Request.Headers.TryGetValue("TenantId", out var tenantIdHeader) ||
                !int.TryParse(tenantIdHeader, out tenantId))
            {
                _logger.LogWarning("TenantId header is missing or invalid.");
                return false;
            }

            return true;
        }

        [HttpPost("Patient-Demographic")]
        public async Task<IActionResult> CreateDemographic([FromBody] PatientDemographicViewModel dto)
        {
            if (!TryGetTenantId(out var tenantId))
                return BadRequest(new { Message = "TenantId header is required", StatusCode = 400 });

            try
            {
                var patientId = await _patientService.CreatePatientDemographicAsync(tenantId, dto);
                _logger.LogInformation("Patient demographic created with ID {PatientId} for Tenant {TenantId}", patientId, tenantId);

                return StatusCode(201, new
                {
                    Data = new { PatientId = patientId },
                    Message = "Patient demographic created successfully",
                    StatusCode = 201
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating patient demographic for Tenant {TenantId}", tenantId);
                return StatusCode(500, new { Message = $"Internal server error: {ex.Message}", StatusCode = 500 });
            }
        }

        [HttpPost("Patient-Contact-Information")]
        public async Task<IActionResult> CreateContact([FromBody] PatientContactViewModel dto)
        {
            if (!TryGetTenantId(out var tenantId))
                return BadRequest(new { Message = "TenantId header is required", StatusCode = 400 });

            try
            {
                var contactId = await _patientService.CreatePatientContactAsync(tenantId, dto);
                _logger.LogInformation("Patient contact created with ID {ContactId} for Tenant {TenantId}", contactId, tenantId);

                return StatusCode(201, new
                {
                    Data = new { ContactId = contactId },
                    Message = "Patient contact created successfully",
                    StatusCode = 201
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating patient contact for Tenant {TenantId}", tenantId);
                return StatusCode(500, new { Message = $"Internal server error: {ex.Message}", StatusCode = 500 });
            }
        }

        [HttpPost("Patient-Emergency-Contact")]
        public async Task<IActionResult> CreateEmergencyContact([FromBody] BulkEmergencyContactViewModel request)
        {
            if (!TryGetTenantId(out var tenantId))
                return BadRequest(new { Message = "TenantId header is required", StatusCode = 400 });

            if (request == null || request.EmergencyContacts == null || !request.EmergencyContacts.Any())
                return BadRequest(new { Message = "Request body with emergency contacts is required", StatusCode = 400 });

            if (!ModelState.IsValid)
                return BadRequest(new { Message = "Validation failed", Errors = ModelState, StatusCode = 400 });

            try
            {
                var contactIds = await _patientService.CreateEmergencyContactsAsync(tenantId, request);
                _logger.LogInformation("Emergency contacts created for Tenant {TenantId}", tenantId);

                return StatusCode(201, new
                {
                    Data = new { ContactIds = contactIds },
                    Message = "Emergency contacts created successfully",
                    StatusCode = 201
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating emergency contacts for Tenant {TenantId}", tenantId);
                return StatusCode(500, new { Message = $"Internal server error: {ex.Message}", StatusCode = 500 });
            }
        }

        [HttpPost("Patient-Insurance-Information")]
        public async Task<IActionResult> CreateInsurance([FromBody] InsuranceInformationViewModel dto)
        {
            if (!TryGetTenantId(out var tenantId))
                return BadRequest(new { Message = "TenantId header is required", StatusCode = 400 });

            try
            {
                var insuranceId = await _patientService.CreateInsuranceInformationAsync(tenantId, dto);
                _logger.LogInformation("Insurance info created with ID {InsuranceId} for Tenant {TenantId}", insuranceId, tenantId);

                return StatusCode(201, new
                {
                    Data = new { InsuranceId = insuranceId },
                    Message = "Insurance information created successfully",
                    StatusCode = 201
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating insurance info for Tenant {TenantId}", tenantId);
                return StatusCode(500, new { Message = $"Internal server error: {ex.Message}", StatusCode = 500 });
            }
        }

        [HttpPost("Patient-Other-Information")]
        public async Task<IActionResult> CreateOtherInformation([FromBody] OtherInformationViewModel dto)
        {
            if (!TryGetTenantId(out var tenantId))
                return BadRequest(new { Message = "TenantId header is required", StatusCode = 400 });

            try
            {
                var otherId = await _patientService.CreateOtherInformationAsync(tenantId, dto);
                _logger.LogInformation("Other information created with ID {OtherInformationId} for Tenant {TenantId}", otherId, tenantId);

                return StatusCode(201, new
                {
                    Data = new { OtherInformationId = otherId },
                    Message = "Other information created successfully",
                    StatusCode = 201
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating other information for Tenant {TenantId}", tenantId);
                return StatusCode(500, new { Message = $"Internal server error: {ex.Message}", StatusCode = 500 });
            }
        }

        [HttpGet("Get-All-Patients")]
        public async Task<IActionResult> GetAllPatients([FromQuery] string? search = null, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            if (!TryGetTenantId(out var tenantId))
                return BadRequest(new { Message = "TenantId header is required", StatusCode = 400 });
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 10;
            if (pageSize > 100) pageSize = 100;
            try
            {
                var result = await _patientService.GetAllPatientsAsync(tenantId, search, page, pageSize);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all patients for Tenant {TenantId}", tenantId);
                return StatusCode(500, new { Message = $"Internal server error: {ex.Message}", StatusCode = 500 });
            }
        }

        [HttpGet("Get-Patient-ById")]
        public async Task<IActionResult> GetPatientById([FromQuery] int patientId)
        {
            if (!TryGetTenantId(out var tenantId))
                return BadRequest(new { Message = "TenantId header is required", StatusCode = 400 });

            try
            {
                var result = await _patientService.GetPatientByIdAsync(tenantId, patientId);
                if (result == null)
                    return NotFound(new { Message = $"Patient with Id {patientId} not found", StatusCode = 404 });

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving patient {PatientId} for Tenant {TenantId}", patientId, tenantId);
                return StatusCode(500, new { Message = $"Internal server error: {ex.Message}", StatusCode = 500 });
            }
        }

        [HttpGet("Get-Patient-Source-Info")]
        public async Task<IActionResult> GetPatientSourceInfo([FromQuery] int patientId)
        {
            if (!TryGetTenantId(out var tenantId))
                return BadRequest(new { Message = "TenantId header is required", StatusCode = 400 });

            try
            {
                var sourceInfo = await _patientService.GetPatientSourceInfoAsync(tenantId, patientId);
                return Ok(new
                {
                    Data = sourceInfo,
                    Message = "Patient source info retrieved successfully",
                    StatusCode = 200
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving patient source info for Patient {PatientId}, Tenant {TenantId}", patientId, tenantId);
                return StatusCode(500, new { Message = $"Internal server error: {ex.Message}", StatusCode = 500 });
            }
        }

        [HttpGet("Get-Sober-Living-Homes")]
        public async Task<IActionResult> GetSoberLivingHomes()
        {
            if (!TryGetTenantId(out var tenantId))
                return BadRequest(new { Message = "TenantId header is required", StatusCode = 400 });

            try
            {
                var soberHomes = await _patientService.GetSoberLivingHomesAsync(tenantId);
                return Ok(new
                {
                    Data = soberHomes,
                    Message = "Sober living homes retrieved successfully",
                    StatusCode = 200
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving sober living homes for Tenant {TenantId}", tenantId);
                return StatusCode(500, new { Message = $"Internal server error: {ex.Message}", StatusCode = 500 });
            }
        }

        [HttpPost("Import-Patient-Records")]
        public async Task<IActionResult> ImportPatients(IFormFile csvFile, [FromForm] int? soberLivingHomeId = null)
        {
            if (!TryGetTenantId(out var tenantId))
                return BadRequest(new { Message = "TenantId header is required", StatusCode = 400 });

            if (csvFile == null || csvFile.Length == 0)
                return BadRequest(new { Message = "CSV file is required", StatusCode = 400 });

            if (!csvFile.FileName.EndsWith(".csv") && !csvFile.FileName.EndsWith(".xlsx"))
                return BadRequest(new { Message = "Only CSV and Excel (.xlsx) files are allowed", StatusCode = 400 });

            try
            {
                using var stream = csvFile.OpenReadStream();
                var (importedCount, errors) = await _patientService.ImportPatientsFromCsvAsync(tenantId, stream, soberLivingHomeId);

                var message = errors.Count > 0
                    ? $"Imported {importedCount} patients with {errors.Count} errors"
                    : $"Successfully imported {importedCount} patients";
                return Ok(new
                {
                    Data = new
                    {
                        ImportedCount = importedCount,
                        ErrorCount = errors.Count,
                        Errors = errors
                    },
                    Message = message,
                    StatusCode = 200
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error importing patients from CSV for Tenant {TenantId}", tenantId);
                return StatusCode(500, new { Message = $"Import failed: {ex.Message}", StatusCode = 500 });
            }
        }

        [HttpDelete("Delete-Patient")]
        public async Task<IActionResult> DeletePatient([FromQuery] int patientId)
        {
            if (!TryGetTenantId(out var tenantId))
                return BadRequest(new { Message = "TenantId header is required", StatusCode = 400 });

            if (patientId <= 0)
                return BadRequest(new { Message = "PatientId is required and must be greater than 0", StatusCode = 400 });

            try
            {
                await _patientService.DeletePatientAsync(tenantId, patientId);
                return Ok(new { Message = "Patient deleted successfully", StatusCode = 200 });
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { Message = ex.Message, StatusCode = 404 });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting patient {PatientId} for Tenant {TenantId}", patientId, tenantId);
                return StatusCode(500, new { Message = $"Internal server error: {ex.Message}", StatusCode = 500 });
            }
        }

        [HttpGet("imported-patients-with-email")]
        public async Task<IActionResult> GetImportedPatientsWithEmail()
        {
            if (!TryGetTenantId(out var tenantId))
                return BadRequest(new { Message = "TenantId header is required", StatusCode = 400 });

            try
            {
                var patients = await _patientService.GetImportedPatientsWithEmailAsync(tenantId);
                return Ok(new
                {
                    Data = patients,
                    Message = $"Found {patients.Count} imported patients with email consent",
                    StatusCode = 200
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = ex.Message, StatusCode = 500 });
            }
        }

        [HttpPut("Update-Patient/{patientId}")]
        public async Task<IActionResult> UpdatePatient(int patientId, [FromBody] PatientUpdateViewModel request)
        {
            if (!TryGetTenantId(out var tenantId))
                return BadRequest(new { Message = "TenantId header is required", StatusCode = 400 });

            if (!ModelState.IsValid)
                return BadRequest(new { Message = "Validation failed", Errors = ModelState, StatusCode = 400 });

            if (patientId <= 0)
                return BadRequest(new { Message = "Valid PatientId is required", StatusCode = 400 });

            // Set PatientId from route parameter
            request.PatientId = patientId;

            try
            {
                var success = await _patientService.UpdatePatientAsync(tenantId, request);
                if (success)
                {
                    _logger.LogInformation("Patient {PatientId} updated successfully for Tenant {TenantId}", request.PatientId, tenantId);
                    return Ok(new
                    {
                        Data = new { PatientId = request.PatientId },
                        Message = "Patient updated successfully",
                        StatusCode = 200
                    });
                }
                else
                {
                    return StatusCode(500, new { Message = "Failed to update patient", StatusCode = 500 });
                }
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { Message = ex.Message, StatusCode = 404 });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating patient {PatientId} for Tenant {TenantId}", request.PatientId, tenantId);
                return StatusCode(500, new { Message = $"Internal server error: {ex.Message}", StatusCode = 500 });
            }
        }
    }
}
