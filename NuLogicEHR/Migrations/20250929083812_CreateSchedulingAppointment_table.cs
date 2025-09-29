using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace NuLogicEHR.Migrations
{
    /// <inheritdoc />
    public partial class CreateSchedulingAppointment_table : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ModifiedBy",
                schema: "public",
                table: "Tenants");

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

            migrationBuilder.CreateIndex(
                name: "IX_SchedulingAppointments_PatientId",
                schema: "public",
                table: "SchedulingAppointments",
                column: "PatientId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SchedulingAppointments",
                schema: "public");

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedBy",
                schema: "public",
                table: "Tenants",
                type: "timestamp with time zone",
                nullable: true);
        }
    }
}
