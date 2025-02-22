using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SoapProductionApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class updateBatchPricesWithTaxes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PriceOfPackageWihoutTax",
                table: "Batches",
                newName: "PriceOfPackageWithoutTax");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PriceOfPackageWithoutTax",
                table: "Batches",
                newName: "PriceOfPackageWihoutTax");
        }
    }
}
