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

            // Create all patient-related tables with relationships
            var createTablesScript = $@"
                CREATE TABLE IF NOT EXISTS ""{schemaName}"".""PatientDemographics"" (
                    ""Id"" SERIAL PRIMARY KEY,
                    ""ProfileImagePath"" TEXT,
                    ""FirstName"" TEXT NOT NULL,
                    ""MiddleName"" TEXT,
                    ""LastName"" TEXT NOT NULL,
                    ""Suffix"" TEXT,
                    ""Nickname"" TEXT,
                    ""GenderAtBirth"" TEXT NOT NULL,
                    ""CurrentGender"" TEXT NOT NULL,
                    ""Pronouns"" TEXT,
                    ""DateOfBirth"" TIMESTAMP WITHOUT TIME ZONE NOT NULL,
                    ""MaritalStatus"" TEXT,
                    ""TimeZone"" TEXT,
                    ""PreferredLanguage"" TEXT NOT NULL,
                    ""Occupation"" TEXT,
                    ""SSN"" INTEGER NOT NULL,
                    ""Race"" TEXT NOT NULL,
                    ""Ethnicity"" TEXT,
                    ""TreatmentType"" TEXT
                );

                CREATE TABLE IF NOT EXISTS ""{schemaName}"".""PatientContactInformation"" (
                    ""Id"" SERIAL PRIMARY KEY,
                    ""MobileNumber"" TEXT NOT NULL,
                    ""HomeNumber"" TEXT,
                    ""Email"" TEXT NOT NULL,
                    ""FaxNumber"" TEXT,
                    ""AddressLine1"" TEXT NOT NULL,
                    ""AddressLine2"" TEXT,
                    ""City"" TEXT NOT NULL,
                    ""State"" TEXT NOT NULL,
                    ""Country"" TEXT NOT NULL,
                    ""ZipCode"" TEXT NOT NULL,
                    ""PatientId"" INTEGER NOT NULL,
                    FOREIGN KEY (""PatientId"") REFERENCES ""{schemaName}"".""PatientDemographics""(""Id"")
                );

                CREATE TABLE IF NOT EXISTS ""{schemaName}"".""EmergencyContacts"" (
                    ""Id"" SERIAL PRIMARY KEY,
                    ""RelationshipWithPatient"" TEXT,
                    ""FirstName"" TEXT,
                    ""LastName"" TEXT,
                    ""PhoneNumber"" TEXT,
                    ""Email"" TEXT,
                    ""PatientId"" INTEGER NOT NULL,
                    FOREIGN KEY (""PatientId"") REFERENCES ""{schemaName}"".""PatientDemographics""(""Id"")
                );

                CREATE TABLE IF NOT EXISTS ""{schemaName}"".""InsuranceInformation"" (
                    ""Id"" SERIAL PRIMARY KEY,
                    ""PaymentMethod"" BOOLEAN NOT NULL,
                    ""InsuranceType"" TEXT,
                    ""InsuranceName"" TEXT,
                    ""MemberId"" TEXT,
                    ""PlanName"" TEXT,
                    ""PlanType"" TEXT,
                    ""GroupId"" TEXT,
                    ""GroupName"" TEXT,
                    ""EffectiveStartDate"" TIMESTAMP WITHOUT TIME ZONE,
                    ""EffectiveEndDate"" TIMESTAMP WITHOUT TIME ZONE,
                    ""PatientRelationshipWithInsured"" TEXT,
                    ""InsuranceCardFilePath"" TEXT,
                    ""PatientId"" INTEGER NOT NULL,
                    FOREIGN KEY (""PatientId"") REFERENCES ""{schemaName}"".""PatientDemographics""(""Id"")
                );

                CREATE TABLE IF NOT EXISTS ""{schemaName}"".""OtherInformation"" (
                    ""Id"" SERIAL PRIMARY KEY,
                    ""ConsentToEmail"" BOOLEAN,
                    ""ConsentToMessage"" BOOLEAN,
                    ""PracticeLocation"" TEXT,
                    ""RegistrationDate"" TIMESTAMP WITHOUT TIME ZONE,
                    ""Source"" TEXT,
                    ""PatientId"" INTEGER NOT NULL,
                    FOREIGN KEY (""PatientId"") REFERENCES ""{schemaName}"".""PatientDemographics""(""Id"")
                );

                CREATE TABLE IF NOT EXISTS ""{schemaName}"".""Patients"" (
                    ""Id"" SERIAL PRIMARY KEY,
                    ""Name"" TEXT NOT NULL,
                    ""DateOfBirth"" TIMESTAMP WITHOUT TIME ZONE NOT NULL,
                    ""Email"" TEXT NOT NULL
                );";

            using var tablesCmd = new Npgsql.NpgsqlCommand(createTablesScript, connection);
            await tablesCmd.ExecuteNonQueryAsync();
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
