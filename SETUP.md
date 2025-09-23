# Multi-Tenant EHR Setup Instructions

## 1. Database Setup
Update the connection string in `appsettings.json` with your PostgreSQL credentials:
```json
"DefaultConnection": "Host=localhost;Database=nulogic_ehr;Username=postgres;Password=your_password"
```

## 2. Run Migrations

### Create and run tenant migrations (public schema):
```bash
dotnet ef migrations add InitialTenantMigration --context TenantContext --output-dir Migrations/Tenant
dotnet ef database update --context TenantContext
```

### Create application migrations template:
```bash
dotnet ef migrations add InitialApplicationMigration --context ApplicationDbContext --output-dir Migrations/Application
dotnet ef database update --context ApplicationDbContext
```

## 3. Usage Examples

### Create a tenant:
```bash
POST /api/tenants
Content-Type: application/json

{
  "name": "Hospital One"
}
```

### Access tenant data:
```bash
GET /api/patients
Headers: X-Tenant-ID: Hospital One
```

### Create patient for specific tenant:
```bash
POST /api/patients
Headers: X-Tenant-ID: Hospital One
Content-Type: application/json

{
  "firstName": "John",
  "lastName": "Doe",
  "email": "john.doe@example.com",
  "dateOfBirth": "1990-01-01T00:00:00Z",
  "phoneNumber": "123-456-7890"
}
```

## 4. Database Structure
- **public schema**: Contains tenant management (Tenants table)
- **tenant schemas**: Each tenant gets their own schema (e.g., hospital_one, clinic_two)
- **Complete isolation**: Each tenant's data is completely separated

## 5. Run the Application
```bash
dotnet run
```

Navigate to `https://localhost:7xxx/swagger` to test the API.
