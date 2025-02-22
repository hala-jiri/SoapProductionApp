using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SoapProductionApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class batchPriceForStockNotMapped : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PriceOfAvailableStockQuantityWithoutTax",
                table: "Batches");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "PriceOfAvailableStockQuantityWithoutTax",
                table: "Batches",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
