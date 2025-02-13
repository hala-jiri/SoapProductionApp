using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SoapProductionApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class ChangeModelWarehouseItemPercentageAndMinimumQuantity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "TaxPercentage",
                table: "WarehouseItems",
                type: "int",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "TaxPercentage",
                table: "WarehouseItems",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");
        }
    }
}
