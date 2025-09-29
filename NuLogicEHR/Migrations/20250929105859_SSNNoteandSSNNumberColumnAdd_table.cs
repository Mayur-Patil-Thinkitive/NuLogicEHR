using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NuLogicEHR.Migrations
{
    /// <inheritdoc />
    public partial class SSNNoteandSSNNumberColumnAdd_table : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SSNNote",
                schema: "public",
                table: "PatientDemographics",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EmailNote",
                schema: "public",
                table: "PatientContactInformation",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SSNNote",
                schema: "public",
                table: "PatientDemographics");

            migrationBuilder.DropColumn(
                name: "EmailNote",
                schema: "public",
                table: "PatientContactInformation");
        }
    }
}
