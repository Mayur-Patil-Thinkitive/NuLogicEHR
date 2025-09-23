using Microsoft.EntityFrameworkCore;
using NuLogicEHR.Configurations;
using NuLogicEHR.Models;
using NuLogicEHR.ViewModels;

namespace NuLogicEHR.Services
{
    public class PatientService
    {
        private readonly TenantService _tenantService;
        private readonly DbContextFactory _dbContextFactory;

        public PatientService(TenantService tenantService, DbContextFactory dbContextFactory)
        {
            _tenantService = tenantService;
            _dbContextFactory = dbContextFactory;
        }

        public async Task<PatientResponseViewModel> CreatePatientAsync(int tenantId, PatientCreateViewModel dto)
        {
            if (!await _tenantService.TenantExistsAsync(tenantId))
                throw new InvalidOperationException("Tenant not found");

            var schema = await _tenantService.GetSchemaByTenantIdAsync(tenantId);
            if (schema == null)
                throw new InvalidOperationException("Tenant schema not found");

            var patient = new Patient
            {
                Name = dto.Name,
                DateOfBirth = dto.DateOfBirth,
                Email = dto.Email
            };

            using var context = _dbContextFactory.CreateContext(schema);
            context.Patients.Add(patient);
            await context.SaveChangesAsync();

            return new PatientResponseViewModel
            {
                Id = patient.Id,
                Name = patient.Name,
                DateOfBirth = patient.DateOfBirth,
                Email = patient.Email,
                TenantId = tenantId
            };
        }

        public async Task<List<PatientResponseViewModel>> GetPatientsAsync(int tenantId)
        {
            if (!await _tenantService.TenantExistsAsync(tenantId))
                throw new InvalidOperationException("Tenant not found");

            var schema = await _tenantService.GetSchemaByTenantIdAsync(tenantId);
            if (schema == null)
                throw new InvalidOperationException("Tenant schema not found");

            using var context = _dbContextFactory.CreateContext(schema);
            return await context.Patients
                .Select(p => new PatientResponseViewModel
                {
                    Id = p.Id,
                    Name = p.Name,
                    DateOfBirth = p.DateOfBirth,
                    Email = p.Email,
                    TenantId = tenantId
                })
                .ToListAsync();
        }
    }
}
