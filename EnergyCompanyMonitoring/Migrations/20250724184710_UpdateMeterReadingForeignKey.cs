using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EnergyCompanyMonitoring.Migrations
{
    /// <inheritdoc />
    public partial class UpdateMeterReadingForeignKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MeterReadings_Accounts_AccountEntityId",
                table: "MeterReadings");

            migrationBuilder.DropIndex(
                name: "IX_MeterReadings_AccountEntityId",
                table: "MeterReadings");

            migrationBuilder.DropColumn(
                name: "AccountEntityId",
                table: "MeterReadings");

            migrationBuilder.AddForeignKey(
                name: "FK_MeterReadings_Accounts_AccountId",
                table: "MeterReadings",
                column: "AccountId",
                principalTable: "Accounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MeterReadings_Accounts_AccountId",
                table: "MeterReadings");

            migrationBuilder.AddColumn<int>(
                name: "AccountEntityId",
                table: "MeterReadings",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_MeterReadings_AccountEntityId",
                table: "MeterReadings",
                column: "AccountEntityId");

            migrationBuilder.AddForeignKey(
                name: "FK_MeterReadings_Accounts_AccountEntityId",
                table: "MeterReadings",
                column: "AccountEntityId",
                principalTable: "Accounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
