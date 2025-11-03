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
        private readonly EmailService _emailService;

        public PatientService(TenantService tenantService, DbContextFactory dbContextFactory, ILogger<PatientService> logger, EmailService emailService)
            : base(tenantService, dbContextFactory)
        {
            _logger = logger;
            _emailService = emailService;
        }

        public async Task<int> CreatePatientDemographicAsync(int tenantId, PatientDemographicViewModel request)
        {
            _logger.LogInformation("CreatePatientDemographicAsync called for TenantId: {TenantId} with Patient Name: {FirstName} {LastName}", tenantId, request.FirstName, request.LastName);

            try
            {
                // Validate SSN or note requirement
                _logger.LogInformation("Validating SSN or note requirement for Patient: {FirstName} {LastName}", request.FirstName, request.LastName);
                ValidateSSNOrNote(request);

                using var context = await GetContextAsync(tenantId);
                _logger.LogInformation("Database context successfully retrieved for TenantId: {TenantId}", tenantId);

                var repository = new PatientRepository(context);
                _logger.LogInformation("PatientRepository initialized for TenantId: {TenantId}", tenantId);

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

                _logger.LogInformation("Creating patient demographic record for {FirstName} {LastName} in TenantId: {TenantId}", request.FirstName, request.LastName, tenantId);

                var result = await repository.AddPatientDemographicAsync(demographic);
                _logger.LogInformation("Patient demographic created successfully with Id: {PatientId} for TenantId: {TenantId}", result, tenantId);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating patient demographic for TenantId: {TenantId}, Patient Name: {FirstName} {LastName}", tenantId, request.FirstName, request.LastName);
                throw;
            }
        }

        public async Task<int> CreatePatientContactAsync(int tenantId, PatientContactViewModel request)
        {
            _logger.LogInformation("CreatePatientContactAsync called for TenantId: {TenantId}, PatientId: {PatientId}", tenantId, request.PatientId);

            try
            {
                // Validate email or note requirement
                _logger.LogInformation("Validating email or note requirement for PatientId: {PatientId}", request.PatientId);
                ValidateEmailOrNote(request);

                using var context = await GetContextAsync(tenantId);
                _logger.LogInformation("Database context successfully retrieved for TenantId: {TenantId}", tenantId);

                var repository = new PatientRepository(context);
                _logger.LogInformation("PatientRepository initialized for TenantId: {TenantId}", tenantId);

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

                _logger.LogInformation("Creating patient contact information for PatientId: {PatientId} in TenantId: {TenantId}", request.PatientId, tenantId);

                var result = await repository.AddPatientContactAsync(contact);
                _logger.LogInformation("Patient contact created successfully with Id: {ContactId} for PatientId: {PatientId}, TenantId: {TenantId}", result, request.PatientId, tenantId);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating patient contact for TenantId: {TenantId}, PatientId: {PatientId}", tenantId, request.PatientId);
                throw;
            }
        }
        public async Task<List<int>> CreateEmergencyContactsAsync(int tenantId, BulkEmergencyContactViewModel request)
        {
            _logger.LogInformation("CreateEmergencyContactsAsync called for TenantId: {TenantId} with PatientId: {PatientId}", tenantId, request.PatientId);

            try
            {
                using var context = await GetContextAsync(tenantId);
                _logger.LogInformation("Database context successfully created for TenantId: {TenantId}", tenantId);

                var repository = new PatientRepository(context);
                var contactIds = new List<int>();

                if (request?.EmergencyContacts == null || !request.EmergencyContacts.Any())
                {
                    _logger.LogWarning("No emergency contacts provided for PatientId: {PatientId} under TenantId: {TenantId}", request?.PatientId, tenantId);
                    return contactIds;
                }

                foreach (var emergencyContact in request.EmergencyContacts)
                {
                    _logger.LogInformation("Processing emergency contact for PatientId: {PatientId}, Name: {FirstName} {LastName}",
                        request.PatientId, emergencyContact.FirstName, emergencyContact.LastName);

                    var emergency = new EmergencyContact
                    {
                        PatientId = request.PatientId,
                        RelationshipWithPatient = emergencyContact.RelationshipWithPatient,
                        FirstName = emergencyContact.FirstName,
                        LastName = emergencyContact.LastName,
                        PhoneNumber = emergencyContact.PhoneNumber,
                        Email = emergencyContact.Email
                    };

                    var id = await repository.AddEmergencyContactAsync(emergency);
                    _logger.LogInformation("Emergency contact created successfully with Id: {Id} for PatientId: {PatientId}", id, request.PatientId);

                    contactIds.Add(id);
                }

                _logger.LogInformation("Successfully created {Count} emergency contacts for PatientId: {PatientId} under TenantId: {TenantId}",
                    contactIds.Count, request.PatientId, tenantId);

                return contactIds;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating emergency contacts for Tenant {TenantId} and PatientId {PatientId}", tenantId, request.PatientId);
                throw;
            }
        }
        public async Task<int> CreateInsuranceInformationAsync(int tenantId, InsuranceInformationViewModel request)
        {
            _logger.LogInformation("CreateInsuranceInformationAsync called for TenantId: {TenantId} with PatientId: {PatientId}", tenantId, request.PatientId);

            try
            {
                using var context = await GetContextAsync(tenantId);
                _logger.LogInformation("Database context successfully created for TenantId: {TenantId}", tenantId);

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
                    EffectiveStartDate = request.EffectiveStartDate.HasValue
                        ? DateTime.SpecifyKind(request.EffectiveStartDate.Value, DateTimeKind.Utc)
                        : null,
                    EffectiveEndDate = request.EffectiveEndDate.HasValue
                        ? DateTime.SpecifyKind(request.EffectiveEndDate.Value, DateTimeKind.Utc)
                        : null,
                    PatientRelationshipWithInsured = request.PatientRelationshipWithInsured,
                    InsuranceCard = request.InsuranceCard,
                    InsuranceCard1 = request.InsuranceCard1,
                    PatientId = request.PatientId
                };

                _logger.LogInformation("Creating insurance information for PatientId: {PatientId} under TenantId: {TenantId}", request.PatientId, tenantId);

                var insuranceId = await repository.AddInsuranceInformationAsync(insurance);

                _logger.LogInformation("Insurance information created successfully with Id: {InsuranceId} for PatientId: {PatientId} under TenantId: {TenantId}",
                    insuranceId, request.PatientId, tenantId);

                return insuranceId;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating insurance information for TenantId: {TenantId}, PatientId: {PatientId}", tenantId, request.PatientId);
                throw;
            }
        }
        public async Task<int> CreateOtherInformationAsync(int tenantId, OtherInformationViewModel request)
        {
            _logger.LogInformation("CreateOtherInformationAsync called for TenantId: {TenantId} with PatientId: {PatientId}", tenantId, request.PatientId);

            try
            {
                using var context = await GetContextAsync(tenantId);
                _logger.LogInformation("Database context successfully created for TenantId: {TenantId}", tenantId);

                var repository = new PatientRepository(context);

                var other = new OtherInformation
                {
                    ConsentToEmail = request.ConsentToEmail,
                    ConsentToMessage = request.ConsentToMessage,
                    PracticeLocation = request.PracticeLocation,
                    RegistrationDate = request.RegistrationDate ?? DateTime.UtcNow,
                    Source = request.Source,
                    SoberLivingHome = request.SoberLivingHome,
                    PatientId = request.PatientId,
                    IsUsingNuLeaseTransportationService = request.IsUsingNuLeaseTransportationService
                };

                _logger.LogInformation("Creating 'OtherInformation' record for PatientId: {PatientId} under TenantId: {TenantId}", request.PatientId, tenantId);

                var otherInfoId = await repository.AddOtherInformationAsync(other);

                _logger.LogInformation("OtherInformation record created successfully with Id: {OtherInformationId} for PatientId: {PatientId} under TenantId: {TenantId}",
                    otherInfoId, request.PatientId, tenantId);

                return otherInfoId;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating other information for TenantId: {TenantId}, PatientId: {PatientId}", tenantId, request.PatientId);
                throw;
            }
        }

        public async Task<object> GetAllPatientsAsync(int tenantId, string? search = null, int page = 1, int pageSize = 10)
        {
            try
            {
                _logger.LogInformation("Started fetching patients for TenantId: {TenantId} | Search: {Search} | Page: {Page} | PageSize: {PageSize}", tenantId, search, page, pageSize);

                using var context = await GetContextAsync(tenantId);
                var repository = new PatientRepository(context);
                var query = context.PatientDemographics.AsQueryable();

                if (!string.IsNullOrWhiteSpace(search))
                {
                    _logger.LogInformation("Applying search filter for patients with keyword: {Search}", search);

                    query = query.Where(p =>
                        p.FirstName.Contains(search) ||
                        p.LastName.Contains(search) ||
                        (p.MiddleName != null && p.MiddleName.Contains(search)) ||
                        (p.Nickname != null && p.Nickname.Contains(search)));
                }

                var totalCount = await query.CountAsync();
                _logger.LogInformation("Total patient count found: {TotalCount}", totalCount);

                var demographics = await query
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                _logger.LogInformation("Fetched {Count} patients for page {Page}", demographics.Count, page);

                var result = new List<PatientCompleteViewModel>();

                foreach (var demographic in demographics)
                {
                    _logger.LogDebug("Fetching detailed info for PatientId: {PatientId}", demographic.Id);

                    var contact = await context.PatientContactInformation.FirstOrDefaultAsync(c => c.PatientId == demographic.Id);
                    var emergencyContacts = await context.EmergencyContacts.Where(e => e.PatientId == demographic.Id).ToListAsync();
                    var insurance = await context.InsuranceInformation.FirstOrDefaultAsync(i => i.PatientId == demographic.Id);
                    var otherInfo = await context.OtherInformation.FirstOrDefaultAsync(o => o.PatientId == demographic.Id);

                    result.Add(new PatientCompleteViewModel
                    {
                        PatientId = demographic.Id,
                        Demographic = new PatientDemographicData
                        {
                            ProfileImagePath = demographic.ProfileImagePath,
                            FirstName = demographic.FirstName,
                            MiddleName = demographic.MiddleName,
                            LastName = demographic.LastName,
                            Suffix = demographic.Suffix,
                            Nickname = demographic.Nickname,
                            GenderAtBirth = demographic.GenderAtBirth,
                            CurrentGender = demographic.CurrentGender,
                            Pronouns = demographic.Pronouns,
                            DateOfBirth = demographic.DateOfBirth,
                            MaritalStatus = demographic.MaritalStatus,
                            TimeZone = demographic.TimeZone,
                            PreferredLanguage = demographic.PreferredLanguage,
                            Occupation = demographic.Occupation,
                            SSN = demographic.SSN,
                            SSNNote = demographic.SSNNote,
                            Race = demographic.Race,
                            Ethnicity = demographic.Ethnicity,
                            TreatmentType = demographic.TreatmentType
                        },
                        Contact = contact == null ? null : new PatientContactData
                        {
                            MobileNumber = contact.MobileNumber,
                            HomeNumber = contact.HomeNumber,
                            Email = contact.Email,
                            EmailNote = contact.EmailNote,
                            FaxNumber = contact.FaxNumber,
                            AddressLine1 = contact.AddressLine1,
                            AddressLine2 = contact.AddressLine2,
                            City = contact.City,
                            State = contact.State,
                            Country = contact.Country,
                            ZipCode = contact.ZipCode
                        },
                        EmergencyContacts = emergencyContacts?.Select(ec => new EmergencyContactData
                        {
                            RelationshipWithPatient = ec.RelationshipWithPatient,
                            FirstName = ec.FirstName,
                            LastName = ec.LastName,
                            PhoneNumber = ec.PhoneNumber,
                            Email = ec.Email
                        }).ToList(),
                        Insurance = insurance == null ? null : new InsuranceData
                        {
                            PaymentMethod = insurance.PaymentMethod.HasValue && insurance.PaymentMethod.Value,
                            InsuranceType = insurance.InsuranceType,
                            InsuranceName = insurance.InsuranceName,
                            MemberId = insurance.MemberId,
                            PlanName = insurance.PlanName,
                            PlanType = insurance.PlanType,
                            GroupId = insurance.GroupId,
                            GroupName = insurance.GroupName,
                            EffectiveStartDate = insurance.EffectiveStartDate,
                            EffectiveEndDate = insurance.EffectiveEndDate,
                            PatientRelationshipWithInsured = insurance.PatientRelationshipWithInsured?.ToString(),
                            InsuranceCard = insurance.InsuranceCard,
                            InsuranceCard1 = insurance.InsuranceCard1
                        },
                        OtherInformation = otherInfo == null ? null : new OtherInfoData
                        {
                            ConsentToEmail = otherInfo.ConsentToEmail ?? false,
                            ConsentToMessage = otherInfo.ConsentToMessage ?? false,
                            PracticeLocation = otherInfo.PracticeLocation,
                            RegistrationDate = otherInfo.RegistrationDate,
                            Source = otherInfo.Source?.ToString(),
                            SoberLivingHome = otherInfo.SoberLivingHome?.ToString()
                        }
                    });
                }

                var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

                _logger.LogInformation("Successfully fetched all patients for TenantId: {TenantId} | TotalPages: {TotalPages}", tenantId, totalPages);

                return new
                {
                    Data = result,
                    TotalCount = totalCount,
                    Page = page,
                    PageSize = pageSize,
                    TotalPages = totalPages,
                    Message = "Patients retrieved successfully",
                    StatusCode = 200
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching all patients for TenantId: {TenantId}", tenantId);
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
        public async Task<object> GetPatientSourceInfoAsync(int tenantId, int patientId)
        {
            _logger.LogInformation("Starting GetPatientSourceInfoAsync for TenantId: {TenantId}, PatientId: {PatientId}", tenantId, patientId);
            try
            {
                using var context = await GetContextAsync(tenantId);
                _logger.LogInformation("Database context initialized successfully for TenantId: {TenantId}", tenantId);

                var otherInfo = await context.OtherInformation
                    .Where(o => o.PatientId == patientId)
                    .Select(o => new
                    {
                        o.Source,
                        o.SoberLivingHomeName,
                        o.RegistrationDate
                    })
                    .FirstOrDefaultAsync();

                if (otherInfo == null)
                {
                    _logger.LogWarning("No source information found for PatientId: {PatientId}, TenantId: {TenantId}", patientId, tenantId);
                    return new { Message = "No source information found for this patient" };
                }

                _logger.LogInformation("Successfully fetched source info for PatientId: {PatientId}, TenantId: {TenantId}", patientId, tenantId);
                return otherInfo;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching patient source info for PatientId: {PatientId}, TenantId: {TenantId}", patientId, tenantId);
                throw;
            }
        }

        public async Task<List<object>> GetSoberLivingHomesAsync(int tenantId)
        {
            try
            {
                using var context = await GetContextAsync(tenantId);
                var soberHomes = await context.SoberLivingHomes
                    .Where(s => s.Status == true)
                    .Select(s => new { Id = s.Id, Name = s.SoberLivingHomeName })
                    .ToListAsync();
                
                return soberHomes.Cast<object>().ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching sober living homes for Tenant {TenantId}", tenantId);
                throw;
            }
        }
        public async Task<(int ImportedCount, List<string> Errors)> ImportPatientsFromCsvAsync(int tenantId, Stream csvStream, int? soberLivingHomeId = null)
        {
            var validationErrors = new List<string>();
            var importedCount = 0;

            try
            {
                using var context = await GetContextAsync(tenantId);
                var repository = new PatientRepository(context);

                // Validate soberLivingHomeId if provided
                if (soberLivingHomeId.HasValue)
                {
                    var soberHomeExists = await context.SoberLivingHomes
                        .AnyAsync(s => s.Id == soberLivingHomeId.Value && s.Status == true);
                    
                    if (!soberHomeExists)
                    {
                        validationErrors.Add($"Invalid SoberLivingHomeId: {soberLivingHomeId.Value}. Please select a valid sober living home.");
                        return (0, validationErrors);
                    }
                } 
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
            "SSN", "TreatmentType", "Email", "InsuranceName", "MemberId"
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

                        // Create PatientContactInformation with email if provided
                        if (!string.IsNullOrEmpty(record.Email))
                        {
                            var contactInfo = new PatientContactInformation
                            {
                                Email = record.Email,
                                MobileNumber = "Null",
                                AddressLine1 = "Null",
                                City = "Null",
                                State = "Null",
                                Country = "Null",
                                ZipCode = "Null",
                                PatientId = patientId
                            };

                            await repository.AddPatientContactAsync(contactInfo);
                        }

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

                        // Always create OtherInformation for imported patients with Source = SoberLivingHome
                        var otherInfo = new OtherInformation
                        {
                            Source = Source.SoberLivingHome,
                            ConsentToEmail = true,
                            ConsentToMessage = true,

                            RegistrationDate = DateTime.UtcNow,
                            PatientId = patientId
                        };

                        // Only set SoberLivingHomeId if it exists and is valid
                        if (soberLivingHomeId.HasValue)
                        {
                            var soberHome = await context.SoberLivingHomes
                                .Where(s => s.Id == soberLivingHomeId.Value && s.Status == true)
                                .Select(s => new { s.Id, s.SoberLivingHomeName })
                                .FirstOrDefaultAsync();
                            
                            if (soberHome != null)
                            {
                                otherInfo.SoberLivingHomeId = soberHome.Id;
                                otherInfo.SoberLivingHomeName = soberHome.SoberLivingHomeName;
                            }
                        }

                        await repository.AddOtherInformationAsync(otherInfo);
                        // Send intake form email if patient has email
                        if (!string.IsNullOrEmpty(record.Email))
                        {
                            var fullName = $"{record.FirstName} {record.LastName}";
                            await _emailService.SendIntakeFormEmailAsync(record.Email, fullName, otherInfo.SoberLivingHomeName);
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
        public async Task DeletePatientAsync(int tenantId, int patientId)
        {
            try
            {
                using var context = await GetContextAsync(tenantId);

                var patient = await context.PatientDemographics.FindAsync(patientId);
                if (patient == null)
                    throw new InvalidOperationException($"Patient with ID {patientId} not found");

                // Delete related records first (cascade delete)
                var contacts = context.PatientContactInformation.Where(c => c.PatientId == patientId);
                var emergencyContacts = context.EmergencyContacts.Where(e => e.PatientId == patientId);
                var insurance = context.InsuranceInformation.Where(i => i.PatientId == patientId);
                var otherInfo = context.OtherInformation.Where(o => o.PatientId == patientId);

                context.PatientContactInformation.RemoveRange(contacts);
                context.EmergencyContacts.RemoveRange(emergencyContacts);
                context.InsuranceInformation.RemoveRange(insurance);
                context.OtherInformation.RemoveRange(otherInfo);
                context.PatientDemographics.Remove(patient);

                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting patient {PatientId} for Tenant {TenantId}", patientId, tenantId);
                throw;
            }
        }
        public async Task<List<object>> GetImportedPatientsWithEmailAsync(int tenantId)
        {
            try
            {
                using var context = await GetContextAsync(tenantId);
                var importedPatients = await context.OtherInformation
                    .Where(o => o.Source == Source.SoberLivingHome && o.ConsentToEmail == true)
                    .Join(context.PatientDemographics,
                        o => o.PatientId,
                        p => p.Id,
                        (o, p) => new { OtherInfo = o, Patient = p })
                    .Join(context.PatientContactInformation,
                        combined => combined.Patient.Id,
                        c => c.PatientId,
                        (combined, c) => new
                        {
                            PatientId = combined.Patient.Id,
                            FirstName = combined.Patient.FirstName,
                            LastName = combined.Patient.LastName,
                            Email = c.Email,
                            SoberLivingHomeName = combined.OtherInfo.SoberLivingHomeName,
                            RegistrationDate = combined.OtherInfo.RegistrationDate,
                            ConsentToEmail = combined.OtherInfo.ConsentToEmail
                        })
                    .Where(x => !string.IsNullOrEmpty(x.Email))
                    .ToListAsync();

                return importedPatients.Cast<object>().ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching imported patients with email for Tenant {TenantId}", tenantId);
                throw;
            }
        }
        public async Task<object?> GetPatientByIdAsync(int tenantId, int patientId)
        {
            try
            {
                using var context = await GetContextAsync(tenantId);

                var demographic = await context.PatientDemographics.FirstOrDefaultAsync(p => p.Id == patientId);
                if (demographic == null) return null;

                var contact = await context.PatientContactInformation.FirstOrDefaultAsync(c => c.PatientId == patientId);
                var emergencyContacts = await context.EmergencyContacts.Where(e => e.PatientId == patientId).ToListAsync();
                var insurance = await context.InsuranceInformation.FirstOrDefaultAsync(i => i.PatientId == patientId);
                var otherInfo = await context.OtherInformation.FirstOrDefaultAsync(o => o.PatientId == patientId);

                var result = new PatientCompleteViewModel
                {
                    PatientId = demographic.Id,
                    Demographic = new PatientDemographicData
                    {
                        ProfileImagePath = demographic.ProfileImagePath,
                        FirstName = demographic.FirstName,
                        MiddleName = demographic.MiddleName,
                        LastName = demographic.LastName,
                        Suffix = demographic.Suffix,
                        Nickname = demographic.Nickname,
                        GenderAtBirth = demographic.GenderAtBirth,
                        CurrentGender = demographic.CurrentGender,
                        Pronouns = demographic.Pronouns,
                        DateOfBirth = demographic.DateOfBirth,
                        MaritalStatus = demographic.MaritalStatus,
                        TimeZone = demographic.TimeZone,
                        PreferredLanguage = demographic.PreferredLanguage,
                        Occupation = demographic.Occupation,
                        SSN = demographic.SSN,
                        SSNNote = demographic.SSNNote,
                        Race = demographic.Race,
                        Ethnicity = demographic.Ethnicity,
                        TreatmentType = demographic.TreatmentType
                    },
                    Contact = contact == null ? null : new PatientContactData
                    {
                        MobileNumber = contact.MobileNumber,
                        HomeNumber = contact.HomeNumber,
                        Email = contact.Email,
                        EmailNote = contact.EmailNote,
                        FaxNumber = contact.FaxNumber,
                        AddressLine1 = contact.AddressLine1,
                        AddressLine2 = contact.AddressLine2,
                        City = contact.City,
                        State = contact.State,
                        Country = contact.Country,
                        ZipCode = contact.ZipCode
                    },
                    EmergencyContacts = emergencyContacts?.Select(ec => new EmergencyContactData
                    {
                        RelationshipWithPatient = ec.RelationshipWithPatient,
                        FirstName = ec.FirstName,
                        LastName = ec.LastName,
                        PhoneNumber = ec.PhoneNumber,
                        Email = ec.Email
                    }).ToList(),
                    Insurance = insurance == null ? null : new InsuranceData
                    {
                        PaymentMethod = insurance.PaymentMethod.HasValue && insurance.PaymentMethod.Value,
                        InsuranceType = insurance.InsuranceType,
                        InsuranceName = insurance.InsuranceName,
                        MemberId = insurance.MemberId,
                        PlanName = insurance.PlanName,
                        PlanType = insurance.PlanType,
                        GroupId = insurance.GroupId,
                        GroupName = insurance.GroupName,
                        EffectiveStartDate = insurance.EffectiveStartDate,
                        EffectiveEndDate = insurance.EffectiveEndDate,
                        PatientRelationshipWithInsured = insurance.PatientRelationshipWithInsured?.ToString(),
                        InsuranceCard = insurance.InsuranceCard,
                        InsuranceCard1 = insurance.InsuranceCard1
                    },
                    OtherInformation = otherInfo == null ? null : new OtherInfoData
                    {
                        ConsentToEmail = otherInfo.ConsentToEmail ?? false,
                        ConsentToMessage = otherInfo.ConsentToMessage ?? false,
                        PracticeLocation = otherInfo.PracticeLocation,
                        RegistrationDate = otherInfo.RegistrationDate,
                        Source = otherInfo.Source?.ToString(),
                        SoberLivingHome = otherInfo.SoberLivingHome?.ToString()
                    }
                };

                return new
                {
                    Data = result,
                    Message = "Patient retrieved successfully",
                    StatusCode = 200
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching patient {PatientId} for Tenant {TenantId}", patientId, tenantId);
                throw;
            }
        }
        public async Task<bool> UpdatePatientAsync(int tenantId, PatientUpdateViewModel request)
        {
            try
            {
                using var context = await GetContextAsync(tenantId);

                var patient = await context.PatientDemographics.FindAsync(request.PatientId);
                if (patient == null)
                    throw new InvalidOperationException($"Patient with ID {request.PatientId} not found");

                if (request.Demographic != null)
                    UpdateDemographic(context, request.PatientId, request.Demographic);

                if (request.Contact != null)
                    await UpdateContactAsync(context, request.PatientId, request.Contact);

                if (request.EmergencyContacts != null)
                    await UpdateEmergencyContactsAsync(context, request.PatientId, request.EmergencyContacts);

                if (request.Insurance != null)
                    await UpdateInsuranceAsync(context, request.PatientId, request.Insurance);

                if (request.OtherInformation != null)
                    await UpdateOtherInfoAsync(context, request.PatientId, request.OtherInformation);

                await context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating patient {PatientId}", request.PatientId);
                throw;
            }
        }
        private void UpdateDemographic(ApplicationDbContext context, int patientId, PatientDemographicUpdateData data)
        {
            _logger.LogInformation("Starting UpdateDemographic for PatientId: {PatientId}", patientId);

            try
            {
                var demo = context.PatientDemographics.FirstOrDefault(p => p.Id == patientId);
                if (demo == null)
                {
                    _logger.LogWarning("No demographic record found for PatientId: {PatientId}", patientId);
                    return;
                }

                _logger.LogInformation("Demographic record found for PatientId: {PatientId}. Updating fields if provided...", patientId);

                if (!string.IsNullOrEmpty(data.FirstName)) demo.FirstName = data.FirstName;
                if (!string.IsNullOrEmpty(data.MiddleName)) demo.MiddleName = data.MiddleName;
                if (!string.IsNullOrEmpty(data.LastName)) demo.LastName = data.LastName;
                if (!string.IsNullOrEmpty(data.Suffix)) demo.Suffix = data.Suffix;
                if (!string.IsNullOrEmpty(data.Nickname)) demo.Nickname = data.Nickname;
                if (data.DateOfBirth.HasValue)
                    demo.DateOfBirth = data.DateOfBirth.Value.Kind == DateTimeKind.Utc
                        ? data.DateOfBirth.Value
                        : data.DateOfBirth.Value.ToUniversalTime();
                if (!string.IsNullOrEmpty(data.GenderAtBirth)) demo.GenderAtBirth = data.GenderAtBirth;
                if (!string.IsNullOrEmpty(data.CurrentGender)) demo.CurrentGender = data.CurrentGender;
                if (!string.IsNullOrEmpty(data.Pronouns)) demo.Pronouns = data.Pronouns;
                if (!string.IsNullOrEmpty(data.MaritalStatus)) demo.MaritalStatus = data.MaritalStatus;
                if (!string.IsNullOrEmpty(data.TimeZone)) demo.TimeZone = data.TimeZone;
                if (!string.IsNullOrEmpty(data.Occupation)) demo.Occupation = data.Occupation;
                if (data.SSN.HasValue) demo.SSN = data.SSN.Value;
                if (!string.IsNullOrEmpty(data.SSNNote)) demo.SSNNote = data.SSNNote;
                if (!string.IsNullOrEmpty(data.Race)) demo.Race = data.Race;
                if (!string.IsNullOrEmpty(data.Ethnicity)) demo.Ethnicity = data.Ethnicity;
                if (!string.IsNullOrEmpty(data.PreferredLanguage)) demo.PreferredLanguage = data.PreferredLanguage;
                if (data.TreatmentType.HasValue)
                    demo.TreatmentType = ((NuLogicEHR.Enums.TreatmentType)data.TreatmentType.Value).ToString();

                demo.ModifiedBy = DateTime.UtcNow;
                context.PatientDemographics.Update(demo);

                _logger.LogInformation("Successfully updated demographic record for PatientId: {PatientId}", patientId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating demographic information for PatientId: {PatientId}", patientId);
                throw;
            }
        }
        private async Task UpdateContactAsync(ApplicationDbContext context, int patientId, PatientContactUpdateData data)
        {
            _logger.LogInformation("Starting UpdateContactAsync for PatientId: {PatientId}", patientId);

            try
            {
                var contact = await context.PatientContactInformation.FirstOrDefaultAsync(c => c.PatientId == patientId);
                if (contact == null)
                {
                    _logger.LogWarning("No contact information found for PatientId: {PatientId}", patientId);
                    return;
                }

                _logger.LogInformation("Contact record found for PatientId: {PatientId}. Updating fields if provided...", patientId);

                if (!string.IsNullOrEmpty(data.MobileNumber)) contact.MobileNumber = data.MobileNumber;
                if (!string.IsNullOrEmpty(data.HomeNumber)) contact.HomeNumber = data.HomeNumber;
                if (!string.IsNullOrEmpty(data.Email)) contact.Email = data.Email;
                if (!string.IsNullOrEmpty(data.EmailNote)) contact.EmailNote = data.EmailNote;
                if (!string.IsNullOrEmpty(data.FaxNumber)) contact.FaxNumber = data.FaxNumber;
                if (!string.IsNullOrEmpty(data.AddressLine1)) contact.AddressLine1 = data.AddressLine1;
                if (!string.IsNullOrEmpty(data.AddressLine2)) contact.AddressLine2 = data.AddressLine2;
                if (!string.IsNullOrEmpty(data.City)) contact.City = data.City;
                if (!string.IsNullOrEmpty(data.State)) contact.State = data.State;
                if (!string.IsNullOrEmpty(data.Country)) contact.Country = data.Country;
                if (!string.IsNullOrEmpty(data.ZipCode)) contact.ZipCode = data.ZipCode;

                contact.ModifiedBy = DateTime.UtcNow;
                context.PatientContactInformation.Update(contact);

                _logger.LogInformation("Successfully updated contact information for PatientId: {PatientId}", patientId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating contact information for PatientId: {PatientId}", patientId);
                throw;
            }
        }
        private async Task UpdateEmergencyContactsAsync(ApplicationDbContext context, int patientId, List<EmergencyContactUpdateData> data)
        {
            _logger.LogInformation("Starting UpdateEmergencyContactsAsync for PatientId: {PatientId}", patientId);

            try
            {
                // Validate patient exists
                var patientExists = await context.PatientDemographics.AnyAsync(p => p.Id == patientId);
                if (!patientExists)
                {
                    _logger.LogWarning("Cannot update emergency contacts. No patient found with PatientId: {PatientId}", patientId);
                    return;
                }

                _logger.LogInformation("Patient found. Fetching existing emergency contacts for PatientId: {PatientId}", patientId);

                var existing = await context.EmergencyContacts.Where(ec => ec.PatientId == patientId).ToListAsync();

                // Replace all existing contacts with new ones
                context.EmergencyContacts.RemoveRange(existing);
                _logger.LogInformation("Existing emergency contacts removed for PatientId: {PatientId}. Processing new contact data...", patientId);

                foreach (var item in data)
                {
                    if (item.IsDeleted && item.Id.HasValue)
                    {
                        var toDelete = existing.FirstOrDefault(ec => ec.Id == item.Id.Value);
                        if (toDelete != null)
                        {
                            context.EmergencyContacts.Remove(toDelete);
                            _logger.LogInformation("Deleted emergency contact (Id: {ContactId}) for PatientId: {PatientId}", item.Id.Value, patientId);
                        }
                        else
                        {
                            _logger.LogWarning("Tried to delete non-existing emergency contact (Id: {ContactId}) for PatientId: {PatientId}", item.Id, patientId);
                        }
                    }
                    else if (item.Id.HasValue)
                    {
                        var toUpdate = existing.FirstOrDefault(ec => ec.Id == item.Id.Value);
                        if (toUpdate != null)
                        {
                            if (!string.IsNullOrEmpty(item.RelationshipWithPatient)) toUpdate.RelationshipWithPatient = item.RelationshipWithPatient;
                            if (!string.IsNullOrEmpty(item.FirstName)) toUpdate.FirstName = item.FirstName;
                            if (!string.IsNullOrEmpty(item.LastName)) toUpdate.LastName = item.LastName;
                            if (!string.IsNullOrEmpty(item.PhoneNumber)) toUpdate.PhoneNumber = item.PhoneNumber;
                            if (!string.IsNullOrEmpty(item.Email)) toUpdate.Email = item.Email;
                            toUpdate.ModifiedBy = DateTime.UtcNow;
                            context.EmergencyContacts.Update(toUpdate);

                            _logger.LogInformation("Updated emergency contact (Id: {ContactId}) for PatientId: {PatientId}", item.Id.Value, patientId);
                        }
                        else
                        {
                            _logger.LogWarning("Tried to update non-existing emergency contact (Id: {ContactId}) for PatientId: {PatientId}", item.Id, patientId);
                        }
                    }
                    else
                    {
                        context.EmergencyContacts.Add(new EmergencyContact
                        {
                            PatientId = patientId,
                            RelationshipWithPatient = item.RelationshipWithPatient,
                            FirstName = item.FirstName,
                            LastName = item.LastName,
                            PhoneNumber = item.PhoneNumber,
                            Email = item.Email,
                            CreatedBy = DateTime.UtcNow,
                            ModifiedBy = DateTime.UtcNow
                        });

                        _logger.LogInformation("Added new emergency contact ({FirstName} {LastName}) for PatientId: {PatientId}", item.FirstName, item.LastName, patientId);
                    }
                }

                _logger.LogInformation("Completed updating emergency contacts for PatientId: {PatientId}", patientId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating emergency contacts for PatientId: {PatientId}", patientId);
                throw;
            }
        }
        private async Task UpdateInsuranceAsync(ApplicationDbContext context, int patientId, InsuranceUpdateData data)
        {
            _logger.LogInformation("Starting UpdateInsuranceAsync for PatientId: {PatientId}", patientId);

            try
            {
                var insurance = await context.InsuranceInformation.FirstOrDefaultAsync(i => i.PatientId == patientId);
                if (insurance == null)
                {
                    _logger.LogWarning("No insurance record found for PatientId: {PatientId}", patientId);
                    return;
                }

                _logger.LogInformation("Insurance record found for PatientId: {PatientId}. Updating fields if provided...", patientId);

                if (data.PaymentMethod.HasValue) insurance.PaymentMethod = data.PaymentMethod.Value;
                if (!string.IsNullOrEmpty(data.InsuranceType)) insurance.InsuranceType = data.InsuranceType;
                if (!string.IsNullOrEmpty(data.InsuranceName)) insurance.InsuranceName = data.InsuranceName;
                if (!string.IsNullOrEmpty(data.MemberId)) insurance.MemberId = data.MemberId;
                if (!string.IsNullOrEmpty(data.PlanName)) insurance.PlanName = data.PlanName;
                if (!string.IsNullOrEmpty(data.PlanType)) insurance.PlanType = data.PlanType;
                if (!string.IsNullOrEmpty(data.GroupId)) insurance.GroupId = data.GroupId;
                if (!string.IsNullOrEmpty(data.GroupName)) insurance.GroupName = data.GroupName;
                if (data.EffectiveStartDate.HasValue)
                    insurance.EffectiveStartDate = data.EffectiveStartDate.Value.Kind == DateTimeKind.Utc
                        ? data.EffectiveStartDate.Value
                        : data.EffectiveStartDate.Value.ToUniversalTime();
                if (data.EffectiveEndDate.HasValue)
                    insurance.EffectiveEndDate = data.EffectiveEndDate.Value.Kind == DateTimeKind.Utc
                        ? data.EffectiveEndDate.Value
                        : data.EffectiveEndDate.Value.ToUniversalTime();
                if (!string.IsNullOrEmpty(data.PatientRelationshipWithInsured))
                    insurance.PatientRelationshipWithInsured = Enum.Parse<NuLogicEHR.Enums.PatientRelationshipWithInsured>(data.PatientRelationshipWithInsured);

                insurance.ModifiedBy = DateTime.UtcNow;
                context.InsuranceInformation.Update(insurance);

                _logger.LogInformation("Successfully updated insurance information for PatientId: {PatientId}", patientId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating insurance information for PatientId: {PatientId}", patientId);
                throw;
            }
        }
        private async Task UpdateOtherInfoAsync(ApplicationDbContext context, int patientId, OtherInfoUpdateData data)
        {
            _logger.LogInformation("Starting UpdateOtherInfoAsync for PatientId: {PatientId}", patientId);

            try
            {
                var other = await context.OtherInformation.FirstOrDefaultAsync(o => o.PatientId == patientId);
                if (other == null)
                {
                    _logger.LogWarning("No 'OtherInformation' record found for PatientId: {PatientId}", patientId);
                    return;
                }

                _logger.LogInformation("Found 'OtherInformation' record for PatientId: {PatientId}. Updating fields if provided...", patientId);

                if (data.ConsentToEmail.HasValue) other.ConsentToEmail = data.ConsentToEmail.Value;
                if (data.ConsentToMessage.HasValue) other.ConsentToMessage = data.ConsentToMessage.Value;
                if (data.SoberLivingHomeId.HasValue) other.SoberLivingHomeId = data.SoberLivingHomeId.Value;
                if (!string.IsNullOrEmpty(data.PracticeLocation)) other.PracticeLocation = data.PracticeLocation;
                if (data.RegistrationDate.HasValue)
                    other.RegistrationDate = data.RegistrationDate.Value.Kind == DateTimeKind.Utc
                        ? data.RegistrationDate.Value
                        : data.RegistrationDate.Value.ToUniversalTime();

                if (data.Source.HasValue)
                {
                    other.Source = (NuLogicEHR.Enums.Source)data.Source.Value;

                    _logger.LogInformation("Updated source to {Source} for PatientId: {PatientId}", data.Source.Value, patientId);

                    // If source is WalkIn (0), clear SoberLivingHome fields
                    if (data.Source.Value == 0)
                    {
                        other.SoberLivingHomeId = null;
                        other.SoberLivingHomeName = null;
                        _logger.LogInformation("Cleared SoberLivingHome details since Source is WalkIn for PatientId: {PatientId}", patientId);
                    }
                }

                if (!string.IsNullOrEmpty(data.SoberLivingHomeName))
                    other.SoberLivingHomeName = data.SoberLivingHomeName;

                other.ModifiedBy = DateTime.UtcNow;
                context.OtherInformation.Update(other);

                _logger.LogInformation("Successfully updated 'OtherInformation' for PatientId: {PatientId}", patientId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating 'OtherInformation' for PatientId: {PatientId}", patientId);
                throw;
            }
        }

    }
}
