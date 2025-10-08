using NuLogicEHR.Common.Exceptions;
using NuLogicEHR.Configurations;
using NuLogicEHR.Models;
using NuLogicEHR.Repository;
using NuLogicEHR.DTOs;
using NuLogicEHR.Enums;
using NuLogicEHR.ViewModels;

namespace NuLogicEHR.Services
{
    public class SettingService : BaseService
    {
        public SettingService(TenantService tenantService, DbContextFactory dbContextFactory)
            : base(tenantService, dbContextFactory)
        {
        }

        public async Task<int> CreateProviderAsync(int tenantId, Provider provider)
        {
            try
            {
                if (provider.KioskAccess == true && !provider.KioskPin.HasValue)
                {
                    throw new CustomValidationException("4-digit PIN is required when Kiosk Access is enabled");
                }

                using var context = await GetContextAsync(tenantId);
                var repository = new SettingRepository(context);
                return await repository.CreateAsync(provider);
            }
            catch (CustomValidationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while creating the provider.", ex);
            }
        }
        public async Task<IEnumerable<Provider>> GetAllProvidersAsync(int tenantId)
        {
            try
            {
                using var context = await GetContextAsync(tenantId);
                var repository = new SettingRepository(context);
                return await repository.GetAllAsync();
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while fetching providers.", ex);
            }
        }

        public async Task<StaffResponseDto> CreateStaffAsync(int tenantId, CreateStaffViewModel req)
        {
            try
            {
                // Validate Role selection
                if (req.Role != 0 && req.Role != 1)
                {
                    throw new ApplicationException("Invalid role. Please select 0 for FrontDesk or 1 for Biller");
                }

                if (req.KioskAccess && !req.KioskPin.HasValue)
                {
                    throw new CustomValidationException("4-digit PIN is required when Kiosk Access is enabled");
                }

                // Validate Biller-specific fields if Role is Biller (1)
                if (req.Role == 1)
                {
                    if (string.IsNullOrEmpty(req.BillerId))
                        throw new ApplicationException("BillerId is required for Biller role");

                    if (string.IsNullOrEmpty(req.TaxIdNumber))
                        throw new ApplicationException("TaxIdNumber is required for Biller role");
                }

                using var context = await GetContextAsync(tenantId);
                var repository = new SettingRepository(context);

                var staff = new Staff
                {
                    Role = (StaffRoleType)req.Role,
                    FirstName = req.FirstName,
                    MiddleName = req.MiddleName,
                    LastName = req.LastName,
                    EmailId = req.EmailId,
                    PhoneNumber = req.PhoneNumber,
                    KioskAccess = req.KioskAccess,
                    KioskPin = req.KioskPin,
                    MailingAddressLine1 = req.MailingAddressLine1,
                    MailingAddressLine2 = req.MailingAddressLine2,
                    BillerId = req.BillerId,
                    TaxIdNumber = !string.IsNullOrEmpty(req.TaxIdNumber) ? int.Parse(req.TaxIdNumber) : null,
                    NationalProviderId = req.NationalProviderId,
                    BillingLicenseNumber = req.BillingLicenseNumber,
                    IssuingState = req.IssuingState,
                    MedicareProviderNumber = !string.IsNullOrEmpty(req.MedicareProviderNumber) ? int.Parse(req.MedicareProviderNumber) : null,
                    ClearinghouseId = req.ClearinghouseId,
                    ERAEFTStatus = req.ERAEFTStatus,
                    PayerAssignedId = req.PayerAssignedId,
                    BankRoutingNumber = req.BankRoutingNumber,
                    AccountNumber = req.AccountNumber,
                    CAQHProviderId = req.CAQHProviderId,
                    Status = req.Status ?? "Active"
                };

                var staffId = await repository.CreateStaffAsync(staff);

                // Only process credentials for Biller role
                if (req.Role == 1 && req.Credentials?.Any() == true)
                {
                    foreach (var credentialDto in req.Credentials)
                    {
                        var credential = new StaffCredential
                        {
                            StaffId = staffId,
                            CredentialType = (CredentialType)credentialDto.CredentialType,
                            CredentialNumber = credentialDto.CredentialNumber
                        };
                        context.StaffCredentials.Add(credential);
                    }
                    await context.SaveChangesAsync();
                }

                return new StaffResponseDto
                {
                    Id = staffId,
                    Message = "Staff created successfully",
                    CreatedAt = staff.CreatedAt
                };
            }
            catch (CustomValidationException)
            {
                throw; // This will preserve the "4-digit PIN is required when Kiosk Access is enabled" message
            }
            catch (ApplicationException)
            {
                throw; // This will preserve role validation messages
            }
            catch (FormatException ex)
            {
                throw new ApplicationException("Invalid number format in one of the fields.", ex);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while creating staff.", ex);
            }
        }


        public async Task<IEnumerable<Staff>> GetAllStaffAsync(int tenantId)
        {
            try
            {
                using var context = await GetContextAsync(tenantId);
                var repository = new SettingRepository(context);
                return await repository.GetAllStaffAsync();
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while fetching staff.", ex);
            }
        }
        public async Task<int> CreateSoberLivingHomeAsync(int tenantId, SoberLivingHomeCreateViewModel req)
        {
            try
            {
                using var context = await GetContextAsync(tenantId);
                var repository = new SettingRepository(context);

                var soberLivingHome = new SoberLivingHome
                {
                    SoberLivingHomeName = req.SoberLivingHomeName,
                    ContactPersonName = req.ContactPersonName,
                    EmailId = req.EmailId,
                    ContactNumber = req.ContactNumber,
                    FaxNumber = req.FaxNumber,
                    RegistrationNumber = req.RegistrationNumber,
                    Transportation = req.Transportation,
                    Status = req.Status,
                    AddressLine1 = req.AddressLine1,
                    AddressLine2 = req.AddressLine2,
                    City = req.City,
                    State = req.State,
                    Country = req.Country,
                    ZipCode = req.ZipCode
                };

                return await repository.CreateSoberLivingHomeAsync(soberLivingHome);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while creating sober living home.", ex);
            }
        }

        public async Task<IEnumerable<SoberLivingHomeResponseViewModel>> GetAllSoberLivingHomesAsync(int tenantId)
        {
            try
            {
                using var context = await GetContextAsync(tenantId);
                var repository = new SettingRepository(context);
                var homes = await repository.GetAllSoberLivingHomesAsync();

                return homes.Select(h => new SoberLivingHomeResponseViewModel
                {
                    Id = h.Id,
                    SoberLivingHomeName = h.SoberLivingHomeName,
                    ContactPersonName = h.ContactPersonName,
                    EmailId = h.EmailId,
                    ContactNumber = h.ContactNumber,
                    FaxNumber = h.FaxNumber,
                    RegistrationNumber = h.RegistrationNumber,
                    Transportation = h.Transportation,
                    Status = h.Status,
                    AddressLine1 = h.AddressLine1,
                    AddressLine2 = h.AddressLine2,
                    City = h.City,
                    State = h.State,
                    Country = h.Country,
                    ZipCode = h.ZipCode,
                    CreatedBy = h.CreatedBy,
                    ModifiedBy = h.ModifiedBy
                });
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while fetching sober living homes.", ex);
            }
        }
    }
}