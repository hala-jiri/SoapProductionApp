using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SoapProductionApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class bigReformOfWarehouseItemAndBatches : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AvailableQuantity",
                table: "Batches");

            migrationBuilder.DropColumn(
                name: "PriceOfPackageWithoutTax",
                table: "Batches");

            migrationBuilder.DropColumn(
                name: "PricePerBaseUnit",
                table: "Batches");

            migrationBuilder.DropColumn(
                name: "QuantityOfPackage",
                table: "Batches");

            migrationBuilder.RenameColumn(
                name: "QuantityInBaseUnit",
                table: "Batches",
                newName: "PriceOfBaseUnitWithoutTax");

            migrationBuilder.AlterColumn<int>(
                name: "TaxPercentage",
                table: "Batches",
                type: "int",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AddColumn<int>(
                name: "AvailableQuantityBaseUnit",
                table: "Batches",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "QuantityBaseUnit",
                table: "Batches",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AvailableQuantityBaseUnit",
                table: "Batches");

            migrationBuilder.DropColumn(
                name: "QuantityBaseUnit",
                table: "Batches");

            migrationBuilder.RenameColumn(
                name: "PriceOfBaseUnitWithoutTax",
                table: "Batches",
                newName: "QuantityInBaseUnit");

            migrationBuilder.AlterColumn<decimal>(
                name: "TaxPercentage",
                table: "Batches",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<decimal>(
                name: "AvailableQuantity",
                table: "Batches",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "PriceOfPackageWithoutTax",
                table: "Batches",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "PricePerBaseUnit",
                table: "Batches",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<double>(
                name: "QuantityOfPackage",
                table: "Batches",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }
    }
}
