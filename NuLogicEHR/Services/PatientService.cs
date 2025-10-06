using Microsoft.EntityFrameworkCore;
using NuLogicEHR.Configurations;
using NuLogicEHR.Models;
using NuLogicEHR.Repository;
using NuLogicEHR.ViewModels;
using NuLogicEHR.Enums;
using System.ComponentModel.DataAnnotations;
using CsvHelper;
using System.Globalization;

namespace NuLogicEHR.Services
{
    public class PatientService : BaseService
    {
        private readonly ILogger<PatientService> _logger;

        public PatientService(TenantService tenantService, DbContextFactory dbContextFactory, ILogger<PatientService> logger)
            : base(tenantService, dbContextFactory)
        {
            _logger = logger;
        }

        public async Task<int> CreatePatientDemographicAsync(int tenantId, PatientDemographicViewModel request)
        {
            try
            {
                // Validate SSN or note requirement
                ValidateSSNOrNote(request);

                using var context = await GetContextAsync(tenantId);
                var repository = new PatientRepository(context);

                var demographic = new PatientDemographic
                {
                    ProfileImagePath = request.ProfileImagePath,
                    FirstName = request.FirstName,
                    MiddleName = request.MiddleName,
                    LastName = request.LastName,
                    Suffix = request.Suffix,
                    Nickname = request.Nickname,
                    GenderAtBirth = request.GenderAtBirth,
                    CurrentGender = request.CurrentGender,
                    Pronouns = request.Pronouns,
                    DateOfBirth = DateTime.SpecifyKind(request.DateOfBirth, DateTimeKind.Utc), //  This line was changed
                    MaritalStatus = request.MaritalStatus,
                    TimeZone = request.TimeZone,
                    PreferredLanguage = request.PreferredLanguage,
                    Occupation = request.Occupation,
                    SSN = request.SSN,
                    SSNNote = request.SSNNote,
                    Race = request.Race,
                    Ethnicity = request.Ethnicity,
                    TreatmentType = request.TreatmentType.HasValue ? ((TreatmentType)request.TreatmentType.Value).ToString() : null
                };

                return await repository.AddPatientDemographicAsync(demographic);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating patient demographic for Tenant {TenantId}", tenantId);
                throw;
            }
        }

        public async Task<int> CreatePatientContactAsync(int tenantId, PatientContactViewModel request)
        {
            try
            {
                // Validate email or note requirement
                ValidateEmailOrNote(request);

                using var context = await GetContextAsync(tenantId);
                var repository = new PatientRepository(context);

                var contact = new PatientContactInformation
                {
                    MobileNumber = request.MobileNumber,
                    HomeNumber = request.HomeNumber,
                    Email = request.Email,
                    EmailNote = request.EmailNote,
                    FaxNumber = request.FaxNumber,
                    AddressLine1 = request.AddressLine1,
                    AddressLine2 = request.AddressLine2,
                    City = request.City,
                    State = request.State,
                    Country = request.Country,
                    ZipCode = request.ZipCode,
                    PatientId = request.PatientId
                };

                return await repository.AddPatientContactAsync(contact);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating patient contact for Tenant {TenantId}", tenantId);
                throw;
            }
        }

        public async Task<List<int>> CreateEmergencyContactsAsync(int tenantId, List<EmergencyContactModelViewModel> requests)
        {
            try
            {
                using var context = await GetContextAsync(tenantId);
                var repository = new PatientRepository(context);

                var contactIds = new List<int>();

                foreach (var request in requests)
                {
                    var emergency = new EmergencyContact
                    {
                        PatientId = request.PatientId,
                        RelationshipWithPatient = request.RelationshipWithPatient,
                        FirstName = request.FirstName,
                        LastName = request.LastName,
                        PhoneNumber = request.PhoneNumber,
                        Email = request.Email
                    };

                    var id = await repository.AddEmergencyContactAsync(emergency);
                    contactIds.Add(id);
                }

                return contactIds;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating emergency contacts for Tenant {TenantId}", tenantId);
                throw;
            }
        }

        public async Task<int> CreateInsuranceInformationAsync(int tenantId, InsuranceInformationViewModel request)
        {
            try
            {
                using var context = await GetContextAsync(tenantId);
                var repository = new PatientRepository(context);

                var insurance = new InsuranceInformation
                {
                    PaymentMethod = request.PaymentMethod == (int)PaymentMethod.SelfPay,
                    InsuranceType = request.InsuranceType,
                    InsuranceName = request.InsuranceName,
                    MemberId = request.MemberId,
                    PlanName = request.PlanName,
                    PlanType = request.PlanType,
                    GroupId = request.GroupId,
                    GroupName = request.GroupName,
                    EffectiveStartDate = request.EffectiveStartDate.HasValue ? DateTime.SpecifyKind(request.EffectiveStartDate.Value, DateTimeKind.Utc) : null,
                    EffectiveEndDate = request.EffectiveEndDate.HasValue ? DateTime.SpecifyKind(request.EffectiveEndDate.Value, DateTimeKind.Utc) : null,
                    PatientRelationshipWithInsured = request.PatientRelationshipWithInsured,
                    InsuranceCard = request.InsuranceCard,
                    InsuranceCard1 = request.InsuranceCard1,

                    PatientId = request.PatientId
                };

                return await repository.AddInsuranceInformationAsync(insurance);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating insurance information for Tenant {TenantId}", tenantId);
                throw;
            }
        }

        public async Task<int> CreateOtherInformationAsync(int tenantId, OtherInformationViewModel request)
        {
            try
            {
                using var context = await GetContextAsync(tenantId);
                var repository = new PatientRepository(context);

                var other = new OtherInformation
                {
                    ConsentToEmail = request.ConsentToEmail,
                    ConsentToMessage = request.ConsentToMessage,
                    PracticeLocation = request.PracticeLocation,
                    RegistrationDate = request.RegistrationDate ?? DateTime.UtcNow,
                    Source = request.Source,
                    SoberLivingHome = request.SoberLivingHome,
                    PatientId = request.PatientId
                };

                return await repository.AddOtherInformationAsync(other);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating other information for Tenant {TenantId}", tenantId);
                throw;
            }
        }

        public async Task<object> GetPatientByIdAsync(int tenantId, int patientId)
        {
            try
            {
                using var context = await GetContextAsync(tenantId);
                var repository = new PatientRepository(context);
                return await repository.GetPatientByIdAsync(patientId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching patient with ID {PatientId} for Tenant {TenantId}", patientId, tenantId);
                throw;
            }
        }

        // Validation Methods
        private void ValidateEmailOrNote(PatientContactViewModel request)
        {
            bool hasEmail = !string.IsNullOrWhiteSpace(request.Email) && request.Email != "null";
            bool hasEmailNote = !string.IsNullOrWhiteSpace(request.EmailNote) && request.EmailNote != "null";

            if (!hasEmail && !hasEmailNote)
            {
                throw new ArgumentException("Either Email ID or Email Note must be provided");
            }

            if (hasEmail && hasEmailNote)
            {
                throw new ArgumentException("Please provide either Email ID or Email Note, not both");
            }

            if (hasEmail)
            {
                var emailAttribute = new EmailAddressAttribute();
                if (!emailAttribute.IsValid(request.Email))
                {
                    throw new ArgumentException("Please enter a valid email address");
                }
            }
        }

        private void ValidateSSNOrNote(PatientDemographicViewModel request)
        {
            bool hasSSN = request.SSN.HasValue && request.SSN.Value > 0;
            bool hasSSNNote = !string.IsNullOrWhiteSpace(request.SSNNote) && request.SSNNote != null;

            if (!hasSSN && !hasSSNNote)
            {
                throw new ArgumentException("Either SSN or SSN Note must be provided");
            }

            if (hasSSN && hasSSNNote)
            {
                throw new ArgumentException("Please provide either SSN or SSN Note, not both");
            }

            if (hasSSN && request.SSN.Value.ToString().Length != 9)
            {
                throw new ArgumentException("SSN must be a 9-digit number");
            }
        }

        public async Task<(int ImportedCount, List<string> Errors)> ImportPatientsFromCsvAsync(int tenantId, Stream csvStream)
        {
            var validationErrors = new List<string>();
            var importedCount = 0;

            try
            {
                using var context = await GetContextAsync(tenantId);
                var repository = new PatientRepository(context);

                using var reader = new StreamReader(csvStream);
                using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
                csv.Context.Configuration.HeaderValidated = null;
                csv.Context.Configuration.MissingFieldFound = null;

                // Read header to check for missing columns
                csv.Read();
                csv.ReadHeader();
                var headerRecord = csv.HeaderRecord;

                // Define required and optional columns
                var requiredColumns = new List<string>
        {
            "FirstName", "LastName", "DateOfBirth", "GenderAtBirth", "CurrentGender"
        };
                var optionalColumns = new List<string>
        {
            "SSN", "TreatmentType", "InsuranceName", "MemberId"
        };
                var allExpectedColumns = requiredColumns.Concat(optionalColumns).ToList();

                // Check for missing columns
                var missingColumns = allExpectedColumns
                    .Where(col => !headerRecord.Any(h => h.Equals(col, StringComparison.OrdinalIgnoreCase)))
                    .ToList();

                if (missingColumns.Any())
                {
                    validationErrors.Add($"Missing columns in CSV: {string.Join(", ", missingColumns)}");
                }

                var records = csv.GetRecords<PatientCsvImportViewModel>().ToList();

                for (int i = 0; i < records.Count; i++)
                {
                    var record = records[i];
                    var rowNumber = i + 2; // CSV rows start at 2 (header is row 1)
                    var rowErrors = new List<string>();

                    try
                    {
                        // Collect all validation errors for this row
                        if (string.IsNullOrWhiteSpace(record.FirstName))
                            rowErrors.Add("FirstName is required");

                        if (string.IsNullOrWhiteSpace(record.LastName))
                            rowErrors.Add("LastName is required");

                        if (string.IsNullOrWhiteSpace(record.GenderAtBirth))
                            rowErrors.Add("GenderAtBirth is required");

                        if (string.IsNullOrWhiteSpace(record.CurrentGender))
                            rowErrors.Add("CurrentGender is required");

                        if (!string.IsNullOrWhiteSpace(record.InsuranceName) &&
                            string.IsNullOrWhiteSpace(record.MemberId))
                            rowErrors.Add("MemberId is required when InsuranceName is provided");

                        if (!string.IsNullOrWhiteSpace(record.MemberId) &&
                            string.IsNullOrWhiteSpace(record.InsuranceName))
                            rowErrors.Add("InsuranceName is required when MemberId is provided");

                        // If there are validation errors, add them all and skip this row
                        if (rowErrors.Any())
                        {
                            validationErrors.Add($"Row {rowNumber}: {string.Join("; ", rowErrors)}");
                            _logger.LogWarning("Skipping row {RowNumber} due to validation errors: {Errors}",
                                rowNumber, string.Join("; ", rowErrors));
                            continue;
                        }

                        // Handle missing DateOfBirth with warning message
                        DateTime dobToSave;
                        if (record.DateOfBirth == default)
                        {
                            dobToSave = DateTime.SpecifyKind(new DateTime(1900, 1, 1), DateTimeKind.Utc);
                            validationErrors.Add($"Row {rowNumber}: DateOfBirth is missing, using default date (1900-01-01)");
                            _logger.LogWarning("Row {RowNumber}: DateOfBirth missing, using default date", rowNumber);
                        }
                        else
                        {
                            dobToSave = DateTime.SpecifyKind(record.DateOfBirth, DateTimeKind.Utc);
                        }

                        // Save to DB
                        var demographic = new PatientDemographic
                        {
                            FirstName = record.FirstName,
                            LastName = record.LastName,
                            DateOfBirth = dobToSave,
                            GenderAtBirth = record.GenderAtBirth,
                            CurrentGender = record.CurrentGender,
                            SSN = record.SSN,
                            TreatmentType = record.TreatmentType,
                            PreferredLanguage = "null",
                            Race = "Not Specified"
                        };

                        var patientId = await repository.AddPatientDemographicAsync(demographic);

                        if (!string.IsNullOrEmpty(record.InsuranceName))
                        {
                            var insurance = new InsuranceInformation
                            {
                                PaymentMethod = null,
                                MemberId = record.MemberId,
                                InsuranceName = record.InsuranceName,
                                PatientId = patientId
                            };

                            await repository.AddInsuranceInformationAsync(insurance);
                        }

                        importedCount++;
                    }
                    catch (Exception exRow)
                    {
                        validationErrors.Add($"Row {rowNumber}: {exRow.Message}");
                        _logger.LogWarning(exRow, "Error processing row {RowNumber}", rowNumber);
                    }
                }

                return (importedCount, validationErrors);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error importing patients from CSV for Tenant {TenantId}", tenantId);
                throw;
            }
        }

    }
}