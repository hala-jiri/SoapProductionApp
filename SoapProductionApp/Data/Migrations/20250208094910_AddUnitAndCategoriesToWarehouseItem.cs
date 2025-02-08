using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SoapProductionApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddUnitAndCategoriesToWarehouseItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Category",
                table: "WarehouseItems");

            migrationBuilder.DropColumn(
                name: "Unit",
                table: "WarehouseItems");

            migrationBuilder.AddColumn<int>(
                name: "UnitId",
                table: "WarehouseItems",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Units",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Units", x => x.Id);
                });

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
                name: "IX_WarehouseItems_UnitId",
                table: "WarehouseItems",
                column: "UnitId");

            migrationBuilder.CreateIndex(
                name: "IX_CategoryWarehouseItem_WarehouseItemsId",
                table: "CategoryWarehouseItem",
                column: "WarehouseItemsId");

            migrationBuilder.AddForeignKey(
                name: "FK_WarehouseItems_Units_UnitId",
                table: "WarehouseItems",
                column: "UnitId",
                principalTable: "Units",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WarehouseItems_Units_UnitId",
                table: "WarehouseItems");

            migrationBuilder.DropTable(
                name: "CategoryWarehouseItem");

            migrationBuilder.DropTable(
                name: "Units");

            migrationBuilder.DropTable(
                name: "Categories");

            migrationBuilder.DropIndex(
                name: "IX_WarehouseItems_UnitId",
                table: "WarehouseItems");

            migrationBuilder.DropColumn(
                name: "UnitId",
                table: "WarehouseItems");

            migrationBuilder.AddColumn<string>(
                name: "Category",
                table: "WarehouseItems",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Unit",
                table: "WarehouseItems",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
