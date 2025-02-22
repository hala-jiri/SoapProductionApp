using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SoapProductionApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class updateModelBatchPricePerPackageWithoutTax : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PriceOfPackage",
                table: "Batches",
                newName: "PriceOfPackageWihoutTax");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PriceOfPackageWihoutTax",
                table: "Batches",
                newName: "PriceOfPackage");
        }
    }
}
