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

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var tenant = new Tenant
                {
                    HospitalName = hospitalName,
                    SchemaName = schemaName,
                    CreatedBy = DateTime.UtcNow
                };

                _context.Tenants.Add(tenant);
                await _context.SaveChangesAsync();

                await CreateSchemaAsync(schemaName);
                
                await transaction.CommitAsync();
                _tenantSchemaCache.TryAdd(tenant.Id, schemaName);

                return tenant;
            }
            catch
            {
                await transaction.RollbackAsync();
                await DropSchemaIfExistsAsync(schemaName);
                await ResetTenantSequenceAsync();
                throw;
            }
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
                    ""DateOfBirth"" TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
                    ""MaritalStatus"" TEXT,
                    ""TimeZone"" TEXT,
                    ""PreferredLanguage"" TEXT NOT NULL,
                    ""Occupation"" TEXT,
                    ""SSN"" INTEGER,
                    ""SSNNote"" TEXT,
                    ""Race"" TEXT NOT NULL,
                    ""Ethnicity"" TEXT,
                    ""TreatmentType"" TEXT,
                    ""CreatedBy""  TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
                    ""ModifiedBy"" TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP

                );

                CREATE TABLE IF NOT EXISTS ""{schemaName}"".""PatientContactInformation"" (
                    ""Id"" SERIAL PRIMARY KEY,
                    ""MobileNumber"" TEXT NOT NULL,
                    ""HomeNumber"" TEXT,
                    ""Email"" TEXT,
                    ""EmailNote"" TEXT,
                    ""FaxNumber"" TEXT,
                    ""AddressLine1"" TEXT NOT NULL,
                    ""AddressLine2"" TEXT,
                    ""City"" TEXT NOT NULL,
                    ""State"" TEXT NOT NULL,
                    ""Country"" TEXT NOT NULL,
                    ""ZipCode"" TEXT NOT NULL,
                    ""PatientId"" INTEGER NOT NULL,
                    ""CreatedBy""  TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
                    ""ModifiedBy"" TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
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
                    ""CreatedBy""  TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
                    ""ModifiedBy"" TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
                    FOREIGN KEY (""PatientId"") REFERENCES ""{schemaName}"".""PatientDemographics""(""Id"")
                );

                CREATE TABLE IF NOT EXISTS ""{schemaName}"".""InsuranceInformation"" (
                    ""Id"" SERIAL PRIMARY KEY,
                    ""PaymentMethod"" BOOLEAN,
                    ""InsuranceType"" TEXT,
                    ""InsuranceName"" TEXT,
                    ""MemberId"" TEXT,
                    ""PlanName"" TEXT,
                    ""PlanType"" TEXT,
                    ""GroupId"" TEXT,
                    ""GroupName"" TEXT,
                    ""EffectiveStartDate"" TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
                    ""EffectiveEndDate"" TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
                    ""PatientRelationshipWithInsured"" TEXT,
                    ""InsuranceCard"" TEXT,
                    ""InsuranceCard1"" TEXT,
                    ""PatientId"" INTEGER NOT NULL,
                    ""CreatedBy""  TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
                    ""ModifiedBy"" TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
                    FOREIGN KEY (""PatientId"") REFERENCES ""{schemaName}"".""PatientDemographics""(""Id"")
                );

                CREATE TABLE IF NOT EXISTS ""{schemaName}"".""OtherInformation"" (
                    ""Id"" SERIAL PRIMARY KEY,
                    ""ConsentToEmail"" BOOLEAN,
                    ""ConsentToMessage"" BOOLEAN,
                    ""PracticeLocation"" TEXT,
                    ""RegistrationDate"" TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
                    ""Source"" INTEGER,
                    ""SoberLivingHome"" INTEGER,
                    ""PatientId"" INTEGER NOT NULL,
                    ""CreatedBy""  TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
                    ""ModifiedBy"" TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
                    FOREIGN KEY (""PatientId"") REFERENCES ""{schemaName}"".""PatientDemographics""(""Id"")
                );

                CREATE TABLE IF NOT EXISTS ""{schemaName}"".""SchedulingAppointments"" (
                    ""Id"" SERIAL PRIMARY KEY,
                    ""AppointmentMode"" BOOLEAN NOT NULL,
                    ""TreatmentType"" INTEGER NOT NULL,
                    ""AppointmentType"" INTEGER NOT NULL,
                    ""Location"" INTEGER NOT NULL,
                    ""Date"" TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
                    ""TimeSlot"" TEXT NOT NULL,
                    ""SelectedForms"" TEXT,
                    ""TransportationService"" BOOLEAN NOT NULL DEFAULT FALSE,
                    ""Status"" TEXT NOT NULL DEFAULT 'Scheduled',
                    ""PatientId"" INTEGER NOT NULL,
                    ""CreatedBy""  TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
                    ""ModifiedBy"" TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
                    FOREIGN KEY (""PatientId"") REFERENCES ""{schemaName}"".""PatientDemographics""(""Id"")
                );

                CREATE TABLE IF NOT EXISTS ""{schemaName}"".""Providers"" (
                    ""Id"" SERIAL PRIMARY KEY,
                    ""ProfileImage"" TEXT,
                    ""FirstName"" TEXT NOT NULL,
                    ""MiddleName"" TEXT,
                    ""LastName"" TEXT NOT NULL,
                    ""EmailId"" TEXT NOT NULL,
                    ""PhoneNumber"" TEXT NOT NULL,
                    ""OfficeFaxNumber"" TEXT,
                    ""Gender"" TEXT NOT NULL,
                    ""ProviderType"" INTEGER NOT NULL,
                    ""Role"" INTEGER NOT NULL,
                    ""NPINumber"" TEXT NOT NULL,
                    ""GroupNPINumber"" TEXT NOT NULL,
                    ""LicensedState"" INTEGER NOT NULL,
                    ""LicenseNumber"" TEXT NOT NULL,
                    ""TaxonomyNumber"" TEXT,
                    ""WorkLocation"" INTEGER NOT NULL,
                    ""InsuranceAccepted"" TEXT,
                    ""YearsOfExperience"" INTEGER NOT NULL,
                    ""DEANumber"" TEXT,
                    ""Status"" TEXT NOT NULL,
                    ""MapRenderingProvider"" BOOLEAN,
                    ""KioskAccess"" BOOLEAN,
                    ""KioskPin"" INTEGER,
                    ""Bio"" TEXT,
                    ""Signature"" TEXT,
                    ""CreatedBy""  TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
                    ""ModifiedBy"" TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP

                );

                CREATE TABLE IF NOT EXISTS ""{schemaName}"".""Staff"" (
                    ""Id"" SERIAL PRIMARY KEY,
                    ""ProfileImage"" TEXT,
                    ""Role"" INTEGER NOT NULL,
                    ""FirstName"" TEXT NOT NULL,
                    ""MiddleName"" TEXT,
                    ""LastName"" TEXT NOT NULL,
                    ""EmailId"" TEXT NOT NULL,
                    ""PhoneNumber"" TEXT NOT NULL,
                    ""KioskAccess"" BOOLEAN NOT NULL DEFAULT FALSE,
                    ""KioskPin"" INTEGER,
                    ""MailingAddressLine1"" TEXT,
                    ""MailingAddressLine2"" TEXT,
                    ""BillerId"" TEXT,
                    ""TaxIdNumber"" INTEGER,
                    ""NationalProviderId"" TEXT,
                    ""BillingLicenseNumber"" TEXT,
                    ""IssuingState"" TEXT,
                    ""MedicareProviderNumber"" INTEGER,
                    ""MedicareProviderDoc"" TEXT,
                    ""ClearinghouseId"" TEXT,
                    ""ERAEFTStatus"" TEXT,
                    ""PayerAssignedId"" TEXT,
                    ""BankRoutingNumber"" TEXT,
                    ""AccountNumber"" TEXT,
                    ""CAQHProviderId"" TEXT,
                    ""LastLogin"" TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
                    ""Status"" TEXT NOT NULL DEFAULT 'Active',
                    ""Signature"" TEXT,
                    ""CreatedBy""  TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
                    ""ModifiedBy"" TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP

                );

                CREATE TABLE IF NOT EXISTS ""{schemaName}"".""StaffCredentials"" (
                    ""Id"" SERIAL PRIMARY KEY,
                    ""CredentialType"" INTEGER NOT NULL,
                    ""CredentialNumber"" TEXT NOT NULL,
                    ""StaffId"" INTEGER NOT NULL,
                    ""CreatedBy""  TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
                    ""ModifiedBy"" TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
                    FOREIGN KEY (""StaffId"") REFERENCES ""{schemaName}"".""Staff""(""Id"") ON DELETE CASCADE
                );
                
                CREATE TABLE IF NOT EXISTS ""{schemaName}"".""SoberLivingHomes"" (
                ""Id"" SERIAL PRIMARY KEY,
                ""SoberLivingHomeName"" TEXT NOT NULL,
                ""ContactPersonName"" TEXT NOT NULL,
                ""EmailId"" TEXT NOT NULL,
                ""ContactNumber"" TEXT NOT NULL,
                ""FaxNumber"" TEXT,
                ""RegistrationNumber"" TEXT,
                ""Transportation"" BOOLEAN,
                ""Status"" BOOLEAN,
                ""AddressLine1"" TEXT,
                ""AddressLine2"" TEXT,
                ""City"" TEXT NOT NULL,
                ""State"" TEXT,
                ""Country"" TEXT,
                ""ZipCode"" TEXT,
                ""CreatedBy""  TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
                ""ModifiedBy"" TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP
               );";

            using var tablesCmd = new Npgsql.NpgsqlCommand(createTablesScript, connection);
            await tablesCmd.ExecuteNonQueryAsync();
        }

        private async Task DropSchemaIfExistsAsync(string schemaName)
        {
            try
            {
                var connectionString = _configuration.GetConnectionString("DefaultConnection");
                using var connection = new Npgsql.NpgsqlConnection(connectionString);
                await connection.OpenAsync();
                
                using var dropCmd = new Npgsql.NpgsqlCommand($"DROP SCHEMA IF EXISTS \"{schemaName}\" CASCADE", connection);
                await dropCmd.ExecuteNonQueryAsync();
            }
            catch
            {
                // Ignore cleanup errors
            }
        }

        private async Task ResetTenantSequenceAsync()
        {
            try
            {
                var connectionString = _configuration.GetConnectionString("DefaultConnection");
                using var connection = new Npgsql.NpgsqlConnection(connectionString);
                await connection.OpenAsync();
                
                using var resetCmd = new Npgsql.NpgsqlCommand(
                    "SELECT setval('\"Tenants_Id_seq\"', (SELECT COALESCE(MAX(\"Id\"), 0) FROM \"Tenants\"))", 
                    connection);
                await resetCmd.ExecuteNonQueryAsync();
            }
            catch
            {
                // Ignore sequence reset errors
            }
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
