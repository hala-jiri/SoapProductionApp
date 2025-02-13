using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SoapProductionApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class updateModelsWithEnumUnits : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WarehouseItems_Units_UnitId",
                table: "WarehouseItems");

            migrationBuilder.DropTable(
                name: "Units");

            migrationBuilder.DropIndex(
                name: "IX_WarehouseItems_UnitId",
                table: "WarehouseItems");

            migrationBuilder.RenameColumn(
                name: "UnitId",
                table: "WarehouseItems",
                newName: "Unit");

            migrationBuilder.AddColumn<int>(
                name: "Unit",
                table: "RecipeIngredient",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Unit",
                table: "RecipeIngredient");

            migrationBuilder.RenameColumn(
                name: "Unit",
                table: "WarehouseItems",
                newName: "UnitId");

            migrationBuilder.CreateTable(
                name: "Units",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Abbreviation = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Units", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WarehouseItems_UnitId",
                table: "WarehouseItems",
                column: "UnitId");

            migrationBuilder.AddForeignKey(
                name: "FK_WarehouseItems_Units_UnitId",
                table: "WarehouseItems",
                column: "UnitId",
                principalTable: "Units",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
