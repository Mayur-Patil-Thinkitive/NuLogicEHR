using Microsoft.EntityFrameworkCore;
using NuLogicEHR.Configurations;
using NuLogicEHR.Models;
using System.Collections.Concurrent;

namespace NuLogicEHR.Services
{
    public class TenantService
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        private static readonly ConcurrentDictionary<int, string> _tenantSchemaCache = new();

        public TenantService(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<Tenant> CreateTenantAsync(string hospitalName)
        {
            var existingTenant = await _context.Tenants
                .FirstOrDefaultAsync(t => t.HospitalName.ToLower() == hospitalName.ToLower());

            if (existingTenant != null)
                throw new InvalidOperationException("Tenant with this hospital name already exists");

            var schemaName = hospitalName.ToLower().Replace(" ", "_").Replace("-", "_");

            var tenant = new Tenant
            {
                HospitalName = hospitalName,
                SchemaName = schemaName,
                CreatedAt = DateTime.UtcNow
            };

            _context.Tenants.Add(tenant);
            await _context.SaveChangesAsync();

            await CreateSchemaAsync(schemaName);
            _tenantSchemaCache.TryAdd(tenant.Id, schemaName);

            return tenant;
        }

        private async Task CreateSchemaAsync(string schemaName)
        {
            var connectionString = _configuration.GetConnectionString("DefaultConnection");

            using var connection = new Npgsql.NpgsqlConnection(connectionString);
            await connection.OpenAsync();
            
            using var schemaCmd = new Npgsql.NpgsqlCommand($"CREATE SCHEMA IF NOT EXISTS \"{schemaName}\"", connection);
            await schemaCmd.ExecuteNonQueryAsync();

            var createPatientsTable = $@"
                CREATE TABLE IF NOT EXISTS ""{schemaName}"".""Patients"" (
                    ""Id"" SERIAL PRIMARY KEY,
                    ""Name"" TEXT NOT NULL,
                    ""DateOfBirth"" TIMESTAMP WITHOUT TIME ZONE NOT NULL,
                    ""Email"" TEXT NOT NULL
                );";

            using var patientsCmd = new Npgsql.NpgsqlCommand(createPatientsTable, connection);
            await patientsCmd.ExecuteNonQueryAsync();
        }

        public async Task<string> GetSchemaByTenantIdAsync(int tenantId)
        {
            if (_tenantSchemaCache.TryGetValue(tenantId, out var cachedSchema))
                return cachedSchema;

            var tenant = await _context.Tenants
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Id == tenantId);

            if (tenant?.SchemaName == null)
                return null;

            _tenantSchemaCache.TryAdd(tenantId, tenant.SchemaName);
            return tenant.SchemaName;
        }

        public async Task<bool> TenantExistsAsync(int tenantId)
        {
            return await _context.Tenants.AnyAsync(t => t.Id == tenantId);
        }
    }
}
