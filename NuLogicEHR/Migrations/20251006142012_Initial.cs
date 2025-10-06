using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace NuLogicEHR.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "public");

            migrationBuilder.CreateTable(
                name: "PatientDemographics",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProfileImagePath = table.Column<string>(type: "text", nullable: true),
                    FirstName = table.Column<string>(type: "text", nullable: false),
                    MiddleName = table.Column<string>(type: "text", nullable: true),
                    LastName = table.Column<string>(type: "text", nullable: false),
                    Suffix = table.Column<string>(type: "text", nullable: true),
                    Nickname = table.Column<string>(type: "text", nullable: true),
                    GenderAtBirth = table.Column<string>(type: "text", nullable: false),
                    CurrentGender = table.Column<string>(type: "text", nullable: false),
                    Pronouns = table.Column<string>(type: "text", nullable: true),
                    DateOfBirth = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    MaritalStatus = table.Column<string>(type: "text", nullable: true),
                    TimeZone = table.Column<string>(type: "text", nullable: true),
                    PreferredLanguage = table.Column<string>(type: "text", nullable: true),
                    Occupation = table.Column<string>(type: "text", nullable: true),
                    SSN = table.Column<int>(type: "integer", nullable: true),
                    SSNNote = table.Column<string>(type: "text", nullable: true),
                    Race = table.Column<string>(type: "text", nullable: true),
                    Ethnicity = table.Column<string>(type: "text", nullable: true),
                    TreatmentType = table.Column<string>(type: "text", nullable: true),
                    CreatedBy = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ModifiedBy = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PatientDemographics", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Patients",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    DateOfBirth = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Patients", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Providers",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProfileImage = table.Column<string>(type: "text", nullable: true),
                    FirstName = table.Column<string>(type: "text", nullable: false),
                    MiddleName = table.Column<string>(type: "text", nullable: true),
                    LastName = table.Column<string>(type: "text", nullable: false),
                    EmailId = table.Column<string>(type: "text", nullable: false),
                    PhoneNumber = table.Column<string>(type: "text", nullable: false),
                    OfficeFaxNumber = table.Column<string>(type: "text", nullable: true),
                    Gender = table.Column<string>(type: "text", nullable: false),
                    ProviderType = table.Column<int>(type: "integer", nullable: false),
                    Role = table.Column<int>(type: "integer", nullable: false),
                    NPINumber = table.Column<string>(type: "text", nullable: false),
                    GroupNPINumber = table.Column<string>(type: "text", nullable: false),
                    LicensedState = table.Column<int>(type: "integer", nullable: false),
                    LicenseNumber = table.Column<string>(type: "text", nullable: false),
                    TaxonomyNumber = table.Column<string>(type: "text", nullable: true),
                    WorkLocation = table.Column<int>(type: "integer", nullable: false),
                    InsuranceAccepted = table.Column<string>(type: "text", nullable: true),
                    YearsOfExperience = table.Column<int>(type: "integer", nullable: false),
                    DEANumber = table.Column<string>(type: "text", nullable: true),
                    Status = table.Column<string>(type: "text", nullable: false),
                    MapRenderingProvider = table.Column<bool>(type: "boolean", nullable: true),
                    KioskAccess = table.Column<bool>(type: "boolean", nullable: true),
                    KioskPin = table.Column<int>(type: "integer", nullable: true),
                    Bio = table.Column<string>(type: "text", nullable: true),
                    Signature = table.Column<string>(type: "text", nullable: true),
                    CreatedBy = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ModifiedBy = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Providers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Staff",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProfileImage = table.Column<string>(type: "text", nullable: true),
                    Role = table.Column<int>(type: "integer", nullable: false),
                    FirstName = table.Column<string>(type: "text", nullable: false),
                    MiddleName = table.Column<string>(type: "text", nullable: true),
                    LastName = table.Column<string>(type: "text", nullable: false),
                    EmailId = table.Column<string>(type: "text", nullable: false),
                    PhoneNumber = table.Column<string>(type: "text", nullable: false),
                    KioskAccess = table.Column<bool>(type: "boolean", nullable: false),
                    KioskPin = table.Column<int>(type: "integer", nullable: true),
                    MailingAddressLine1 = table.Column<string>(type: "text", nullable: true),
                    MailingAddressLine2 = table.Column<string>(type: "text", nullable: true),
                    BillerId = table.Column<string>(type: "text", nullable: true),
                    TaxIdNumber = table.Column<int>(type: "integer", nullable: true),
                    NationalProviderId = table.Column<string>(type: "text", nullable: true),
                    BillingLicenseNumber = table.Column<string>(type: "text", nullable: true),
                    IssuingState = table.Column<string>(type: "text", nullable: true),
                    MedicareProviderNumber = table.Column<int>(type: "integer", nullable: true),
                    MedicareProviderDoc = table.Column<string>(type: "text", nullable: true),
                    ClearinghouseId = table.Column<string>(type: "text", nullable: true),
                    ERAEFTStatus = table.Column<string>(type: "text", nullable: true),
                    PayerAssignedId = table.Column<string>(type: "text", nullable: true),
                    BankRoutingNumber = table.Column<string>(type: "text", nullable: true),
                    AccountNumber = table.Column<string>(type: "text", nullable: true),
                    CAQHProviderId = table.Column<string>(type: "text", nullable: true),
                    LastLogin = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Status = table.Column<string>(type: "text", nullable: false),
                    Signature = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Staff", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Tenants",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    HospitalName = table.Column<string>(type: "text", nullable: false),
                    SchemaName = table.Column<string>(type: "text", nullable: false),
                    CreatedBy = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tenants", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EmergencyContacts",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RelationshipWithPatient = table.Column<string>(type: "text", nullable: true),
                    FirstName = table.Column<string>(type: "text", nullable: true),
                    LastName = table.Column<string>(type: "text", nullable: true),
                    PhoneNumber = table.Column<string>(type: "text", nullable: true),
                    Email = table.Column<string>(type: "text", nullable: true),
                    PatientId = table.Column<int>(type: "integer", nullable: false),
                    CreatedBy = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ModifiedBy = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmergencyContacts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmergencyContacts_PatientDemographics_PatientId",
                        column: x => x.PatientId,
                        principalSchema: "public",
                        principalTable: "PatientDemographics",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InsuranceInformation",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PaymentMethod = table.Column<bool>(type: "boolean", nullable: true),
                    InsuranceType = table.Column<string>(type: "text", nullable: true),
                    InsuranceName = table.Column<string>(type: "text", nullable: true),
                    MemberId = table.Column<string>(type: "text", nullable: true),
                    PlanName = table.Column<string>(type: "text", nullable: true),
                    PlanType = table.Column<string>(type: "text", nullable: true),
                    GroupId = table.Column<string>(type: "text", nullable: true),
                    GroupName = table.Column<string>(type: "text", nullable: true),
                    EffectiveStartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    EffectiveEndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    PatientRelationshipWithInsured = table.Column<string>(type: "text", nullable: true),
                    InsuranceCard = table.Column<string>(type: "text", nullable: true),
                    InsuranceCard1 = table.Column<string>(type: "text", nullable: true),
                    PatientId = table.Column<int>(type: "integer", nullable: false),
                    CreatedBy = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ModifiedBy = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InsuranceInformation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InsuranceInformation_PatientDemographics_PatientId",
                        column: x => x.PatientId,
                        principalSchema: "public",
                        principalTable: "PatientDemographics",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OtherInformation",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ConsentToEmail = table.Column<bool>(type: "boolean", nullable: true),
                    ConsentToMessage = table.Column<bool>(type: "boolean", nullable: true),
                    PracticeLocation = table.Column<string>(type: "text", nullable: true),
                    RegistrationDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Source = table.Column<int>(type: "integer", nullable: true),
                    SoberLivingHome = table.Column<int>(type: "integer", nullable: true),
                    PatientId = table.Column<int>(type: "integer", nullable: false),
                    CreatedBy = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ModifiedBy = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OtherInformation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OtherInformation_PatientDemographics_PatientId",
                        column: x => x.PatientId,
                        principalSchema: "public",
                        principalTable: "PatientDemographics",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PatientContactInformation",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    MobileNumber = table.Column<string>(type: "text", nullable: false),
                    HomeNumber = table.Column<string>(type: "text", nullable: true),
                    Email = table.Column<string>(type: "text", nullable: true),
                    EmailNote = table.Column<string>(type: "text", nullable: true),
                    FaxNumber = table.Column<string>(type: "text", nullable: true),
                    AddressLine1 = table.Column<string>(type: "text", nullable: false),
                    AddressLine2 = table.Column<string>(type: "text", nullable: true),
                    City = table.Column<string>(type: "text", nullable: false),
                    State = table.Column<string>(type: "text", nullable: false),
                    Country = table.Column<string>(type: "text", nullable: false),
                    ZipCode = table.Column<string>(type: "text", nullable: false),
                    PatientId = table.Column<int>(type: "integer", nullable: false),
                    CreatedBy = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ModifiedBy = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PatientContactInformation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PatientContactInformation_PatientDemographics_PatientId",
                        column: x => x.PatientId,
                        principalSchema: "public",
                        principalTable: "PatientDemographics",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SchedulingAppointments",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AppointmentMode = table.Column<bool>(type: "boolean", nullable: false),
                    TreatmentType = table.Column<int>(type: "integer", nullable: false),
                    AppointmentType = table.Column<int>(type: "integer", nullable: false),
                    Location = table.Column<int>(type: "integer", nullable: false),
                    Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    TimeSlot = table.Column<string>(type: "text", nullable: false),
                    SelectedForms = table.Column<string>(type: "text", nullable: false),
                    TransportationService = table.Column<bool>(type: "boolean", nullable: false),
                    PatientId = table.Column<int>(type: "integer", nullable: false),
                    CreatedBy = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModifiedBy = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SchedulingAppointments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SchedulingAppointments_PatientDemographics_PatientId",
                        column: x => x.PatientId,
                        principalSchema: "public",
                        principalTable: "PatientDemographics",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StaffCredentials",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CredentialType = table.Column<int>(type: "integer", nullable: false),
                    CredentialNumber = table.Column<string>(type: "text", nullable: false),
                    StaffId = table.Column<int>(type: "integer", nullable: false),
                    CreatedBy = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ModifiedBy = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StaffCredentials", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StaffCredentials_Staff_StaffId",
                        column: x => x.StaffId,
                        principalSchema: "public",
                        principalTable: "Staff",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EmergencyContacts_PatientId",
                schema: "public",
                table: "EmergencyContacts",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_InsuranceInformation_PatientId",
                schema: "public",
                table: "InsuranceInformation",
                column: "PatientId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OtherInformation_PatientId",
                schema: "public",
                table: "OtherInformation",
                column: "PatientId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PatientContactInformation_PatientId",
                schema: "public",
                table: "PatientContactInformation",
                column: "PatientId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SchedulingAppointments_PatientId",
                schema: "public",
                table: "SchedulingAppointments",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_StaffCredentials_StaffId",
                schema: "public",
                table: "StaffCredentials",
                column: "StaffId");

            migrationBuilder.CreateIndex(
                name: "IX_Tenants_HospitalName",
                schema: "public",
                table: "Tenants",
                column: "HospitalName",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EmergencyContacts",
                schema: "public");

            migrationBuilder.DropTable(
                name: "InsuranceInformation",
                schema: "public");

            migrationBuilder.DropTable(
                name: "OtherInformation",
                schema: "public");

            migrationBuilder.DropTable(
                name: "PatientContactInformation",
                schema: "public");

            migrationBuilder.DropTable(
                name: "Patients",
                schema: "public");

            migrationBuilder.DropTable(
                name: "Providers",
                schema: "public");

            migrationBuilder.DropTable(
                name: "SchedulingAppointments",
                schema: "public");

            migrationBuilder.DropTable(
                name: "StaffCredentials",
                schema: "public");

            migrationBuilder.DropTable(
                name: "Tenants",
                schema: "public");

            migrationBuilder.DropTable(
                name: "PatientDemographics",
                schema: "public");

            migrationBuilder.DropTable(
                name: "Staff",
                schema: "public");
        }
    }
}
