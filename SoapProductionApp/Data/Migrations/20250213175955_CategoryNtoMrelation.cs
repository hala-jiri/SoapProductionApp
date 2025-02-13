using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SoapProductionApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class CategoryNtoMrelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Categories_WarehouseItems_WarehouseItemId",
                table: "Categories");

            migrationBuilder.DropIndex(
                name: "IX_Categories_WarehouseItemId",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "WarehouseItemId",
                table: "Categories");

            migrationBuilder.CreateTable(
                name: "CategoryWarehouseItem",
                columns: table => new
                {
                    CategoriesId = table.Column<int>(type: "int", nullable: false),
                    WarehouseItemsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoryWarehouseItem", x => new { x.CategoriesId, x.WarehouseItemsId });
                    table.ForeignKey(
                        name: "FK_CategoryWarehouseItem_Categories_CategoriesId",
                        column: x => x.CategoriesId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CategoryWarehouseItem_WarehouseItems_WarehouseItemsId",
                        column: x => x.WarehouseItemsId,
                        principalTable: "WarehouseItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CategoryWarehouseItem_WarehouseItemsId",
                table: "CategoryWarehouseItem",
                column: "WarehouseItemsId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CategoryWarehouseItem");

            migrationBuilder.AddColumn<int>(
                name: "WarehouseItemId",
                table: "Categories",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Categories_WarehouseItemId",
                table: "Categories",
                column: "WarehouseItemId");

            migrationBuilder.AddForeignKey(
                name: "FK_Categories_WarehouseItems_WarehouseItemId",
                table: "Categories",
                column: "WarehouseItemId",
                principalTable: "WarehouseItems",
                principalColumn: "Id");
        }
    }
}
