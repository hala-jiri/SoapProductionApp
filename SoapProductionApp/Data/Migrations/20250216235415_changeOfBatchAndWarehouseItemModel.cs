using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SoapProductionApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class changeOfBatchAndWarehouseItemModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AvailableQuantityBaseUnit",
                table: "Batches");

            migrationBuilder.RenameColumn(
                name: "Unit",
                table: "Batches",
                newName: "PreferedUnit");

            migrationBuilder.RenameColumn(
                name: "QuantityBaseUnit",
                table: "Batches",
                newName: "AvailableQuantityInBaseUnit");

            migrationBuilder.RenameColumn(
                name: "PriceOfBaseUnitWithoutTax",
                table: "Batches",
                newName: "PricePerPreferUnitWithoutTax");

            migrationBuilder.AddColumn<decimal>(
                name: "AvailableQuantityInPreferUnit",
                table: "Batches",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "PriceOfAvailableStockWithoutTax",
                table: "Batches",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "PricePerBaseUnitWithoutTax",
                table: "Batches",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AvailableQuantityInPreferUnit",
                table: "Batches");

            migrationBuilder.DropColumn(
                name: "PriceOfAvailableStockWithoutTax",
                table: "Batches");

            migrationBuilder.DropColumn(
                name: "PricePerBaseUnitWithoutTax",
                table: "Batches");

            migrationBuilder.RenameColumn(
                name: "PricePerPreferUnitWithoutTax",
                table: "Batches",
                newName: "PriceOfBaseUnitWithoutTax");

            migrationBuilder.RenameColumn(
                name: "PreferedUnit",
                table: "Batches",
                newName: "Unit");

            migrationBuilder.RenameColumn(
                name: "AvailableQuantityInBaseUnit",
                table: "Batches",
                newName: "QuantityBaseUnit");

            migrationBuilder.AddColumn<int>(
                name: "AvailableQuantityBaseUnit",
                table: "Batches",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
