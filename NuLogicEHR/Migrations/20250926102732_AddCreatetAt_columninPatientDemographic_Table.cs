using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NuLogicEHR.Migrations
{
    /// <inheritdoc />
    public partial class AddCreatetAt_columninPatientDemographic_Table : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                schema: "public",
                table: "PatientDemographics",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                schema: "public",
                table: "PatientDemographics");
        }
    }
}
