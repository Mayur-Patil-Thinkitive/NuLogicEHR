using NuLogicEHR.Configurations;
using Microsoft.EntityFrameworkCore;
using NuLogicEHR.Models;
using NuLogicEHR.ViewModels;

namespace NuLogicEHR.Services
{
    public abstract class BaseService
    {
        protected readonly TenantService _tenantService;
        protected readonly DbContextFactory _dbContextFactory;

        protected BaseService(TenantService tenantService, DbContextFactory dbContextFactory)
        {
            _tenantService = tenantService;
            _dbContextFactory = dbContextFactory;
        }
        protected async Task<ApplicationDbContext> GetContextAsync(int tenantId)
        {
            var schema = await _tenantService.GetSchemaByTenantIdAsync(tenantId);
            if (schema == null) throw new InvalidOperationException("Tenant not found");
            return _dbContextFactory.CreateContext(schema);
        }
    }
}
