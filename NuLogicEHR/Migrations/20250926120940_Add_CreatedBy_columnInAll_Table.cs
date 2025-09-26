using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NuLogicEHR.Migrations
{
    /// <inheritdoc />
    public partial class Add_CreatedBy_columnInAll_Table : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                schema: "public",
                table: "Tenants",
                newName: "CreatedBy");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                schema: "public",
                table: "PatientDemographics",
                newName: "ModifiedBy");

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedBy",
                schema: "public",
                table: "Tenants",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedBy",
                schema: "public",
                table: "PatientDemographics",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedBy",
                schema: "public",
                table: "PatientContactInformation",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedBy",
                schema: "public",
                table: "PatientContactInformation",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedBy",
                schema: "public",
                table: "OtherInformation",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedBy",
                schema: "public",
                table: "OtherInformation",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedBy",
                schema: "public",
                table: "InsuranceInformation",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedBy",
                schema: "public",
                table: "InsuranceInformation",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedBy",
                schema: "public",
                table: "EmergencyContacts",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedBy",
                schema: "public",
                table: "EmergencyContacts",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ModifiedBy",
                schema: "public",
                table: "Tenants");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                schema: "public",
                table: "PatientDemographics");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                schema: "public",
                table: "PatientContactInformation");

            migrationBuilder.DropColumn(
                name: "ModifiedBy",
                schema: "public",
                table: "PatientContactInformation");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                schema: "public",
                table: "OtherInformation");

            migrationBuilder.DropColumn(
                name: "ModifiedBy",
                schema: "public",
                table: "OtherInformation");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                schema: "public",
                table: "InsuranceInformation");

            migrationBuilder.DropColumn(
                name: "ModifiedBy",
                schema: "public",
                table: "InsuranceInformation");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                schema: "public",
                table: "EmergencyContacts");

            migrationBuilder.DropColumn(
                name: "ModifiedBy",
                schema: "public",
                table: "EmergencyContacts");

            migrationBuilder.RenameColumn(
                name: "CreatedBy",
                schema: "public",
                table: "Tenants",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "ModifiedBy",
                schema: "public",
                table: "PatientDemographics",
                newName: "CreatedAt");
        }
    }
}
