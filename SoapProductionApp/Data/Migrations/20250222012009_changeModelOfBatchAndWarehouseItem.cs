using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SoapProductionApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class changeModelOfBatchAndWarehouseItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BaseUnit",
                table: "WarehouseItems");

            migrationBuilder.DropColumn(
                name: "DefaultUnit",
                table: "WarehouseItems");

            migrationBuilder.DropColumn(
                name: "Supplier",
                table: "WarehouseItems");

            migrationBuilder.DropColumn(
                name: "AvailableQuantityInBaseUnit",
                table: "Batches");

            migrationBuilder.DropColumn(
                name: "AvailableQuantityInPreferUnit",
                table: "Batches");

            migrationBuilder.DropColumn(
                name: "PreferedUnit",
                table: "Batches");

            migrationBuilder.RenameColumn(
                name: "MinQuantity",
                table: "WarehouseItems",
                newName: "MinimumQuantityAlarm");

            migrationBuilder.RenameColumn(
                name: "PricePerPreferUnitWithoutTax",
                table: "Batches",
                newName: "UnitPriceWithoutTax");

            migrationBuilder.RenameColumn(
                name: "PricePerBaseUnitWithoutTax",
                table: "Batches",
                newName: "PriceOfAvailableStockQuantityWithoutTax");

            migrationBuilder.RenameColumn(
                name: "PriceOfAvailableStockWithoutTax",
                table: "Batches",
                newName: "AvailableQuantity");

            migrationBuilder.AlterColumn<string>(
                name: "Supplier",
                table: "Batches",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "MinimumQuantityAlarm",
                table: "WarehouseItems",
                newName: "MinQuantity");

            migrationBuilder.RenameColumn(
                name: "UnitPriceWithoutTax",
                table: "Batches",
                newName: "PricePerPreferUnitWithoutTax");

            migrationBuilder.RenameColumn(
                name: "PriceOfAvailableStockQuantityWithoutTax",
                table: "Batches",
                newName: "PricePerBaseUnitWithoutTax");

            migrationBuilder.RenameColumn(
                name: "AvailableQuantity",
                table: "Batches",
                newName: "PriceOfAvailableStockWithoutTax");

            migrationBuilder.AddColumn<int>(
                name: "BaseUnit",
                table: "WarehouseItems",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "DefaultUnit",
                table: "WarehouseItems",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Supplier",
                table: "WarehouseItems",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "Supplier",
                table: "Batches",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AvailableQuantityInBaseUnit",
                table: "Batches",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "AvailableQuantityInPreferUnit",
                table: "Batches",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "PreferedUnit",
                table: "Batches",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
