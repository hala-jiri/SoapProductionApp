using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SoapProductionApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class EditCookingModelName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cooking_Recipes_RecipeId",
                table: "Cooking");

            migrationBuilder.DropForeignKey(
                name: "FK_CookingIngredients_Cooking_CookingId",
                table: "CookingIngredients");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Cooking",
                table: "Cooking");

            migrationBuilder.RenameTable(
                name: "Cooking",
                newName: "Cookings");

            migrationBuilder.RenameIndex(
                name: "IX_Cooking_RecipeId",
                table: "Cookings",
                newName: "IX_Cookings_RecipeId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Cookings",
                table: "Cookings",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CookingIngredients_Cookings_CookingId",
                table: "CookingIngredients",
                column: "CookingId",
                principalTable: "Cookings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Cookings_Recipes_RecipeId",
                table: "Cookings",
                column: "RecipeId",
                principalTable: "Recipes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CookingIngredients_Cookings_CookingId",
                table: "CookingIngredients");

            migrationBuilder.DropForeignKey(
                name: "FK_Cookings_Recipes_RecipeId",
                table: "Cookings");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Cookings",
                table: "Cookings");

            migrationBuilder.RenameTable(
                name: "Cookings",
                newName: "Cooking");

            migrationBuilder.RenameIndex(
                name: "IX_Cookings_RecipeId",
                table: "Cooking",
                newName: "IX_Cooking_RecipeId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Cooking",
                table: "Cooking",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Cooking_Recipes_RecipeId",
                table: "Cooking",
                column: "RecipeId",
                principalTable: "Recipes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CookingIngredients_Cooking_CookingId",
                table: "CookingIngredients",
                column: "CookingId",
                principalTable: "Cooking",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
