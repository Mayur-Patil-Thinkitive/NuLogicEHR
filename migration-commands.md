# Migration Commands for Schema-Based Multi-Tenant Setup

## 1. Create Tenant Management Migration (Public Schema)
```bash
dotnet ef migrations add InitialTenantMigration --context TenantContext --output-dir Migrations/Tenant
```

## 2. Update Database with Tenant Management Tables
```bash
dotnet ef database update --context TenantContext
```

## 3. Create Application Migration Template
```bash
dotnet ef migrations add InitialApplicationMigration --context ApplicationDbContext --output-dir Migrations/Application
```

## 4. Apply Migration to Default Schema (for template)
```bash
dotnet ef database update --context ApplicationDbContext
```

## Usage Instructions

### 1. Setup Database
- Create a single PostgreSQL database: `nulogic_ehr`
- Run migration commands 1-4 above

### 2. Create Tenants
```json
POST /api/tenants
{
  "name": "Hospital One"
}
```
This automatically creates schema `hospital_one`

### 3. Access Tenant Data
```
GET /api/patients
Headers: X-Tenant-ID: Hospital One
```

## Database Structure
```
nulogic_ehr (database)
├── public (schema)
│   └── Tenants (table)
├── hospital_one (schema)
│   └── Patients (table)
├── clinic_two (schema)
│   └── Patients (table)
└── ... (more tenant schemas)
```

## Benefits of Schema-Based Approach
- Single database connection
- Easier backup and maintenance
- Better resource utilization
- Simpler deployment
- Cross-tenant reporting possible (if needed)

## Migration for New Tenants
When a new tenant is created, the schema is automatically created. 
To apply migrations to existing tenant schemas:

```bash
# For each existing tenant schema
dotnet ef database update --context ApplicationDbContext --connection "Host=localhost;Database=nulogic_ehr;Username=postgres;Password=your_password;SearchPath=tenant_schema_name"
```
