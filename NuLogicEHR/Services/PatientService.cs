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
                    DateOfBirth = request.DateOfBirth,
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

        public async Task<int> CreateEmergencyContactAsync(int tenantId, EmergencyContactModelViewModel request)
        {
            try
            {
                using var context = await GetContextAsync(tenantId);
                var repository = new PatientRepository(context);

                var emergency = new EmergencyContact
                {
                    RelationshipWithPatient = request.RelationshipWithPatient,
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    PhoneNumber = request.PhoneNumber,
                    Email = request.Email,
                    PatientId = request.PatientId
                };

                return await repository.AddEmergencyContactAsync(emergency);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating emergency contact for Tenant {TenantId}", tenantId);
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
                    EffectiveStartDate = request.EffectiveStartDate,
                    EffectiveEndDate = request.EffectiveEndDate,
                    PatientRelationshipWithInsured = request.PatientRelationshipWithInsured,
                    InsuranceCardFilePath = request.InsuranceCardFilePath,
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
            bool hasSSNNote = !string.IsNullOrWhiteSpace(request.SSNNote) && request.SSNNote != "null";

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

        public async Task<int> ImportPatientsFromCsvAsync(int tenantId, Stream csvStream)
        {
            try
            {
                using var context = await GetContextAsync(tenantId);
                var repository = new PatientRepository(context);
                
                using var reader = new StreamReader(csvStream);
                using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
                
                var records = csv.GetRecords<PatientCsvImportViewModel>().ToList();
                int importedCount = 0;

                foreach (var record in records)
                {
                    // Create demographic
                    var demographic = new PatientDemographic
                    {
                        FirstName = record.FirstName,
                        LastName = record.LastName,
                        DateOfBirth = DateTime.SpecifyKind(record.DateOfBirth, DateTimeKind.Utc),
                        GenderAtBirth = record.GenderAtBirth,
                        CurrentGender = record.CurrentGender,
                        SSN = record.SSN,
                        TreatmentType = record.TreatmentType,
                        PreferredLanguage = "English",
                        Race = "Not Specified"
                    };

                    var patientId = await repository.AddPatientDemographicAsync(demographic);

                    // Create insurance if data provided
                    if (!string.IsNullOrEmpty(record.InsuranceType) || !string.IsNullOrEmpty(record.InsuranceName))
                    {
                        var insurance = new InsuranceInformation
                        {
                            PaymentMethod = null,
                            InsuranceType = record.InsuranceType,
                            InsuranceName = record.InsuranceName,
                            MemberId = record.MemberId,
                            PlanName = record.PlanName,
                            PlanType = record.PlanType,
                            GroupId = record.GroupId,
                            GroupName = record.GroupName,
                            EffectiveStartDate = record.EffectiveStartDate.HasValue ? DateTime.SpecifyKind(record.EffectiveStartDate.Value, DateTimeKind.Utc) : null,
                            EffectiveEndDate = record.EffectiveEndDate.HasValue ? DateTime.SpecifyKind(record.EffectiveEndDate.Value, DateTimeKind.Utc) : null,
                            PatientId = patientId
                        };

                        await repository.AddInsuranceInformationAsync(insurance);
                    }

                    importedCount++;
                }

                return importedCount;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error importing patients from CSV for Tenant {TenantId}", tenantId);
                throw;
            }
        }
    }
}
