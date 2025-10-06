using Microsoft.AspNetCore.Mvc;
using NuLogicEHR.Services;
using NuLogicEHR.ViewModels;

namespace NuLogicEHR.Controllers
{
    [Route("api/v1")]
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

        [HttpPost("patient-demographic")]
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

        [HttpPost("patient-contact-information")]
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

        [HttpPost("patient-emergency-contact")]
        public async Task<IActionResult> CreateEmergencyContact([FromBody] List<EmergencyContactModelViewModel> request)
        {
            if (!TryGetTenantId(out var tenantId))
                return BadRequest(new { Message = "TenantId header is required", StatusCode = 400 });

            if (request == null || !request.Any())
                return BadRequest(new { Message = "Request body is required and cannot be empty", StatusCode = 400 });

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

        [HttpPost("patient-insurance-information")]
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

        [HttpPost("patient-other-information")]
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

        [HttpGet("get-patient-details")]
        public async Task<IActionResult> GetPatient()
        {
            try
            {
                if (!Request.Headers.TryGetValue("TenantId", out var tenantIdHeader) ||
                    !int.TryParse(tenantIdHeader, out var tenantId))
                {
                    return BadRequest(new { Message = "TenantId header is required", StatusCode = 400 });
                }

                if (!Request.Headers.TryGetValue("PatientId", out var patientIdHeader) ||
                    !int.TryParse(patientIdHeader, out var patientId))
                {
                    return BadRequest(new { Message = "PatientId header is required", StatusCode = 400 });
                }

                var patient = await _patientService.GetPatientByIdAsync(tenantId, patientId);
                return Ok(new { Data = patient, Message = "Patient retrieved successfully", StatusCode = 200 });
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

        [HttpPost("import-patient-records")]
        public async Task<IActionResult> ImportPatients(IFormFile csvFile)
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
                var (importedCount, errors) = await _patientService.ImportPatientsFromCsvAsync(tenantId, stream);

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
    }
}