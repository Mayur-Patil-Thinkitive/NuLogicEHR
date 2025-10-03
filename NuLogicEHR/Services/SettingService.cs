using NuLogicEHR.Common.Exceptions;
using NuLogicEHR.Configurations;
using NuLogicEHR.Models;
using NuLogicEHR.Repository;

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
                if (provider.KioskAccess == true && !provider.NumericPin.HasValue)
                {
                    throw new ProviderValidationException("4-digit PIN is required when Kiosk Access is enabled");
                }

                using var context = await GetContextAsync(tenantId);
                var repository = new SettingRepository(context);
                return await repository.CreateAsync(provider);
            }
            catch (ProviderValidationException) // rethrow as is
            {
                throw;
            }
            catch (Exception ex)
            {
                // Log here
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

    }
}
