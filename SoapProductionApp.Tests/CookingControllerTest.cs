using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Infrastructure;
using SoapProductionApp.Controllers;
using SoapProductionApp.Data;
using SoapProductionApp.Models.Recipe;
using SoapProductionApp.Models.Recipe.ViewModels;
using SoapProductionApp.Models.Warehouse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace SoapProductionApp.Tests
{
    public class CookingControllerTest
    {
        private ApplicationDbContext GetDatabaseContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "CookingTestDatabase")
                .Options;

            var dbContext = new ApplicationDbContext(options);
            dbContext.Database.EnsureDeleted(); // Každý test začíná s prázdnou DB
            dbContext.Database.EnsureCreated();
            return dbContext;
        }
        /*
         * Test:
         *        Index - getting list
         *        Create - Check database for cooking.count, and used ingrediences (warehouseItem batches get low), try two variants - taking from one batch vs taking from two batches of warehouse. Check prices.
         *        Edit - Posible to edit just CookingNotes! nothing else.
         *        Delete - Check cooking count and warehouseItem batches should be still low.
         *        ExportToPdf - check if I am getting file (pdf)
         *        CutConfirm - check attributes of cooking (isCut, batchSize - possible to change, BatchSizeWasChanged)
         *       
         * TODO: maybe make one metode to fill all template data to DB (to recude code redundacy)
         */

        [Theory]
        [InlineData(10, 5, 20, 7, 8, 40, 2, 20)]  // First batch size = 10, first batch price per unit = 5, Second batch size = 20, Second batch price per unit = 7, Recipe usage = 8, expected Price per batch = 8*5 = 40
        [InlineData(10, 5, 20, 7, 12, 64, 0, 18)]  // First batch size = 10, first batch price per unit = 5, Second batch size = 20, Second batch price per unit = 7, Recipe usage = 12, expected Price per batch = 10*5+2*7 = 64
        public async Task Create_AddsCookingAndReducaWarehouseItemsStockRedirectsToIndex(int firstBatchSize, decimal firstbatchPricePerUnit, int secondBatchSize, decimal secondbatchPricePerUnit, int recipeUsage, decimal expectedTotalPrice, int leftQuantityBatch1, int leftQuantityBatch2)
        {
            // Arrange
            var dbContext = GetDatabaseContext();
            var controller = new CookingController(dbContext);

            // prepare warehouse stock
            var warehouseItem = new WarehouseItem() { Name = "Test warehouse Item", Unit = Models.UnitMeasurement.UnitType.L, TaxPercentage = 21, MinimumQuantityAlarm = 1, Notes = "Note for test warehouse Item" };
            dbContext.WarehouseItems.Add(warehouseItem);
            await dbContext.SaveChangesAsync();

            var batch1 = new Batch() { Name = "Test Batch name",    PurchaseDate = DateTime.Now, TaxPercentage = 21, Supplier = "test supplier",    WarehouseItemId = warehouseItem.Id, WarehouseItem = warehouseItem, AvailableQuantity = firstBatchSize,  UnitPriceWithoutTax = firstbatchPricePerUnit };
            var batch2 = new Batch() { Name = "Test Batch name 2",  PurchaseDate = DateTime.Now, TaxPercentage = 21, Supplier = "test supplier 2",  WarehouseItemId = warehouseItem.Id, WarehouseItem = warehouseItem, AvailableQuantity = secondBatchSize, UnitPriceWithoutTax = secondbatchPricePerUnit };
            dbContext.Batches.Add(batch1);
            dbContext.Batches.Add(batch2);
            await dbContext.SaveChangesAsync();

            // prepare recipe
            var recipeVM = new RecipeCreateEditViewModel() { ImageUrl = "-", Name = "Test", Note = "Test note", BatchSize = 11, DaysOfCure = 60, ProductType = ProductType.Soap };
            var recipe = new Recipe(recipeVM, new List<RecipeIngredient>());
            dbContext.Recipes.Add(recipe);
            await dbContext.SaveChangesAsync();

            var recipeIngrediet = new RecipeIngredient() { Quantity = recipeUsage, Unit = Models.UnitMeasurement.UnitType.L, Recipe = recipe, RecipeId = recipe.Id, WarehouseItem = warehouseItem, WarehouseItemId = warehouseItem.Id };
            recipe.Ingredients.Add(recipeIngrediet);
            dbContext.RecipeIngredients.Add(recipeIngrediet);
            dbContext.Recipes.Update(recipe);
            await dbContext.SaveChangesAsync();


            // Act
            var beforeCount = await dbContext.Cookings.CountAsync();
            var totalQuantityWarehouseItemBeforeCookingDb = (await dbContext.WarehouseItems.SingleOrDefaultAsync(x => x.Id == warehouseItem.Id))?.TotalAvailableQuantity ?? 0;

            var result = await controller.Create(recipe.Id, recipe.BatchSize, "no notes") as RedirectToActionResult;
            
            var afterCount = await dbContext.Cookings.CountAsync();
            var pricePerCooking = (await dbContext.Cookings.SingleOrDefaultAsync(x => x.RecipeId == recipe.Id))?.TotalCost ?? 0;
            var batch1LeftQuantityDb = (await dbContext.Batches.SingleOrDefaultAsync(x => x.Id == batch1.Id))?.AvailableQuantity ?? 0;
            var batch2LeftQuantityDb = (await dbContext.Batches.SingleOrDefaultAsync(x => x.Id == batch2.Id))?.AvailableQuantity ?? 0;
            var totalQuantityWarehouseItemAfterCookingDb = (await dbContext.WarehouseItems.SingleOrDefaultAsync(x => x.Id == warehouseItem.Id))?.TotalAvailableQuantity ?? 0;


            // Assert
            Assert.Equal("Index", result.ActionName);
            Assert.Equal(beforeCount + 1, afterCount);

            Assert.Equal(expectedTotalPrice, pricePerCooking);
            Assert.Equal(leftQuantityBatch1, batch1LeftQuantityDb);
            Assert.Equal(leftQuantityBatch2, batch2LeftQuantityDb);
            Assert.Equal(totalQuantityWarehouseItemBeforeCookingDb, totalQuantityWarehouseItemAfterCookingDb + recipeUsage);
        }

        [Theory]
        [InlineData(10, 20, 35)]  // First batch size = 10, Second batch size = 20, Recipe usage = 35
        [InlineData(10, 0, 11)]  
        public async Task Create_AddsCookingNotAllowedByLowStockRedirectsToIndex(int firstBatchSize, int secondBatchSize, int recipeUsage)
        {
            // Arrange
            var dbContext = GetDatabaseContext();
            var controller = new CookingController(dbContext);

            // prepare warehouse stock
            var warehouseItem = new WarehouseItem() { Name = "Test warehouse Item", Unit = Models.UnitMeasurement.UnitType.L, TaxPercentage = 21, MinimumQuantityAlarm = 1, Notes = "Note for test warehouse Item" };
            dbContext.WarehouseItems.Add(warehouseItem);
            await dbContext.SaveChangesAsync();

            var batch1 = new Batch() { Name = "Test Batch name", PurchaseDate = DateTime.Now, TaxPercentage = 21, Supplier = "test supplier", WarehouseItemId = warehouseItem.Id, WarehouseItem = warehouseItem, AvailableQuantity = firstBatchSize, UnitPriceWithoutTax = 10 };
            var batch2 = new Batch() { Name = "Test Batch name 2", PurchaseDate = DateTime.Now, TaxPercentage = 21, Supplier = "test supplier 2", WarehouseItemId = warehouseItem.Id, WarehouseItem = warehouseItem, AvailableQuantity = secondBatchSize, UnitPriceWithoutTax = 10 };
            dbContext.Batches.Add(batch1);
            dbContext.Batches.Add(batch2);
            await dbContext.SaveChangesAsync();

            // prepare recipe
            var recipeVM = new RecipeCreateEditViewModel() { ImageUrl = "-", Name = "Test", Note = "Test note", BatchSize = 11, DaysOfCure = 60, ProductType = ProductType.Soap };
            var recipe = new Recipe(recipeVM, new List<RecipeIngredient>());
            dbContext.Recipes.Add(recipe);
            await dbContext.SaveChangesAsync();

            var recipeIngrediet = new RecipeIngredient() { Quantity = recipeUsage, Unit = Models.UnitMeasurement.UnitType.L, Recipe = recipe, RecipeId = recipe.Id, WarehouseItem = warehouseItem, WarehouseItemId = warehouseItem.Id };
            recipe.Ingredients.Add(recipeIngrediet);
            dbContext.RecipeIngredients.Add(recipeIngrediet);
            dbContext.Recipes.Update(recipe);
            await dbContext.SaveChangesAsync();


            // Act
            var beforeCount = await dbContext.Cookings.CountAsync();
            var totalQuantityWarehouseItemBeforeCookingDb = (await dbContext.WarehouseItems.SingleOrDefaultAsync(x => x.Id == warehouseItem.Id))?.TotalAvailableQuantity ?? 0;

            var result = await controller.Create(recipe.Id, recipe.BatchSize, "no notes") as RedirectToActionResult;

            var afterCount = await dbContext.Cookings.CountAsync();
            var pricePerCooking = (await dbContext.Cookings.SingleOrDefaultAsync(x => x.RecipeId == recipe.Id))?.TotalCost ?? 0;
            var batch1LeftQuantityDb = (await dbContext.Batches.SingleOrDefaultAsync(x => x.Id == batch1.Id))?.AvailableQuantity ?? 0;
            var batch2LeftQuantityDb = (await dbContext.Batches.SingleOrDefaultAsync(x => x.Id == batch2.Id))?.AvailableQuantity ?? 0;
            var totalQuantityWarehouseItemAfterCookingDb = (await dbContext.WarehouseItems.SingleOrDefaultAsync(x => x.Id == warehouseItem.Id))?.TotalAvailableQuantity ?? 0;


            // Assert
            Assert.Equal("Index", result.ActionName);
            Assert.True(beforeCount == afterCount, "The count of cooking before and after should be the same, as there was not enough stock available to start cooking.");

            Assert.True(firstBatchSize == batch1LeftQuantityDb, $"Expected {firstBatchSize} but found {batch1LeftQuantityDb} - Quantity mismatch in batch 1. \nIt seems that cooking took place even though it shouldn't have.");
            Assert.True(secondBatchSize == batch2LeftQuantityDb, $"Expected {secondBatchSize} but found {batch2LeftQuantityDb} - Quantity mismatch in batch 2. \nIt seems that cooking took place even though it shouldn't have.");
            Assert.True(totalQuantityWarehouseItemBeforeCookingDb == totalQuantityWarehouseItemAfterCookingDb, $"Expected {totalQuantityWarehouseItemBeforeCookingDb} but found {totalQuantityWarehouseItemAfterCookingDb} - \nIt looks like some quantity was deducted from totalAvailableQuantity even though it shouldn't have been."); // value should be the same because there is NO cooking

        }

        [Fact]
        public async Task Edit_UpdatesCookingCorrectly()
        {
            // Arrange
            var dbContext = GetDatabaseContext();
            var controller = new CookingController(dbContext);

            // prepare warehouse stock
            var warehouseItem = new WarehouseItem() { Name = "Test warehouse Item", Unit = Models.UnitMeasurement.UnitType.L, TaxPercentage = 21, MinimumQuantityAlarm = 1, Notes = "Note for test warehouse Item" };
            dbContext.WarehouseItems.Add(warehouseItem);
            await dbContext.SaveChangesAsync();

            var batch1 = new Batch() { Name = "Test Batch name", PurchaseDate = DateTime.Now, TaxPercentage = 21, Supplier = "test supplier", WarehouseItemId = warehouseItem.Id, WarehouseItem = warehouseItem, AvailableQuantity = 10, UnitPriceWithoutTax = 5 };
            var batch2 = new Batch() { Name = "Test Batch name 2", PurchaseDate = DateTime.Now, TaxPercentage = 21, Supplier = "test supplier 2", WarehouseItemId = warehouseItem.Id, WarehouseItem = warehouseItem, AvailableQuantity = 10, UnitPriceWithoutTax = 5 };
            dbContext.Batches.Add(batch1);
            dbContext.Batches.Add(batch2);
            await dbContext.SaveChangesAsync();

            // prepare recipe
            var recipeVM = new RecipeCreateEditViewModel() { ImageUrl = "-", Name = "Test", Note = "Test note", BatchSize = 11, DaysOfCure = 60 , ProductType = ProductType.Soap };
            var recipe = new Recipe(recipeVM, new List<RecipeIngredient>());
            dbContext.Recipes.Add(recipe);
            await dbContext.SaveChangesAsync();

            var recipeIngrediet = new RecipeIngredient() { Quantity = 5, Unit = Models.UnitMeasurement.UnitType.L, Recipe = recipe, RecipeId = recipe.Id, WarehouseItem = warehouseItem, WarehouseItemId = warehouseItem.Id };
            recipe.Ingredients.Add(recipeIngrediet);
            dbContext.RecipeIngredients.Add(recipeIngrediet);
            dbContext.Recipes.Update(recipe);
            await dbContext.SaveChangesAsync();

            var cooking = new Models.Cooking.Cooking() { BatchSize = 10, CookingDate = DateTime.Now, Recipe = recipe, RecipeId = recipe.Id, RecipeNotes = "Old recipe notes", CookingNotes = "Cooking notes", CuringDate = DateTime.Now.AddDays(5), IsCut = false };
            dbContext.Cookings.Add(cooking);
            await dbContext.SaveChangesAsync();

            var originalBatchSize = cooking.BatchSize;
            var originalRecipeNotes = cooking.RecipeNotes;
            var originalCuringDate = cooking.CuringDate;

            // Act
            cooking.CookingNotes = "Test";

            var result = await controller.Edit(cooking.Id, cooking) as RedirectToActionResult;
            var cookingAfterUpdate = await dbContext.Cookings.FindAsync(cooking.Id);

            // Assert
            Assert.Equal("Index", result.ActionName);
            Assert.Equal("Test", cookingAfterUpdate.CookingNotes);
        }


        [Fact]
        public async Task Edit_UpdatesCookingIncorectly()
        {  // other changes than CookingNotes cannot be changed (no diff)
            /*
             * NOTE: this test is failing due to EF follow the changes
             */

            // Arrange
            var dbContext = GetDatabaseContext();
            var controller = new CookingController(dbContext);

            // prepare warehouse stock
            var warehouseItem = new WarehouseItem() { Name = "Test warehouse Item", Unit = Models.UnitMeasurement.UnitType.L, TaxPercentage = 21, MinimumQuantityAlarm = 1, Notes = "Note for test warehouse Item" };
            dbContext.WarehouseItems.Add(warehouseItem);
            await dbContext.SaveChangesAsync();

            var batch1 = new Batch() { Name = "Test Batch name", PurchaseDate = DateTime.Now, TaxPercentage = 21, Supplier = "test supplier", WarehouseItemId = warehouseItem.Id, WarehouseItem = warehouseItem, AvailableQuantity = 10, UnitPriceWithoutTax = 5 };
            var batch2 = new Batch() { Name = "Test Batch name 2", PurchaseDate = DateTime.Now, TaxPercentage = 21, Supplier = "test supplier 2", WarehouseItemId = warehouseItem.Id, WarehouseItem = warehouseItem, AvailableQuantity = 10, UnitPriceWithoutTax = 5 };
            dbContext.Batches.Add(batch1);
            dbContext.Batches.Add(batch2);
            await dbContext.SaveChangesAsync();

            // prepare recipe
            var recipeVM = new RecipeCreateEditViewModel() { ImageUrl = "-", Name = "Test", Note = "Test note", BatchSize = 11, DaysOfCure = 60 , ProductType = ProductType.Soap };
            var recipe = new Recipe(recipeVM, new List<RecipeIngredient>());
            dbContext.Recipes.Add(recipe);
            await dbContext.SaveChangesAsync();

            var recipeIngrediet = new RecipeIngredient() { Quantity = 5, Unit = Models.UnitMeasurement.UnitType.L, Recipe = recipe, RecipeId = recipe.Id, WarehouseItem = warehouseItem, WarehouseItemId = warehouseItem.Id };
            recipe.Ingredients.Add(recipeIngrediet);
            dbContext.RecipeIngredients.Add(recipeIngrediet);
            dbContext.Recipes.Update(recipe);
            await dbContext.SaveChangesAsync();

            var cooking = new Models.Cooking.Cooking() { BatchSize = 10, CookingDate = DateTime.Now, Recipe = recipe, RecipeId = recipe.Id, RecipeNotes = "Old recipe notes", CookingNotes = "Cooking notes", CuringDate = DateTime.Now.AddDays(5), IsCut = false };
            dbContext.Cookings.Add(cooking);
            await dbContext.SaveChangesAsync();

            var originalBatchSize = cooking.BatchSize;
            var originalRecipeNotes = cooking.RecipeNotes;
            var originalCuringDate = cooking.CuringDate;

            // Act
            cooking.BatchSize = 15;
            cooking.RecipeNotes = "Test";
            cooking.CuringDate = DateTime.Now.AddDays(-10);

            var result = await controller.Edit(cooking.Id, cooking) as RedirectToActionResult;
            var cookingAfterUpdate = await dbContext.Cookings.FindAsync(cooking.Id);

            // Assert
            Assert.Equal("Index", result.ActionName);
            Assert.Equal(originalBatchSize, cookingAfterUpdate.BatchSize);
            Assert.Equal(originalRecipeNotes, cookingAfterUpdate.RecipeNotes);
            Assert.True(originalCuringDate >= cookingAfterUpdate.CuringDate, $"The curing date was updated even though it should not have been.");
        }

        [Fact]
        public async Task Delete_RemovesCookingRecordButKeepConsumedIngrediencesReducedAndRedirectsToIndex()
        {
            // Arrange
            var dbContext = GetDatabaseContext();
            var controller = new CookingController(dbContext);

            // prepare warehouse stock
            var warehouseItem = new WarehouseItem() { Name = "Test warehouse Item", Unit = Models.UnitMeasurement.UnitType.L, TaxPercentage = 21, MinimumQuantityAlarm = 1, Notes = "Note for test warehouse Item" };
            dbContext.WarehouseItems.Add(warehouseItem);
            await dbContext.SaveChangesAsync();

            var batch1 = new Batch() { Name = "Test Batch name", PurchaseDate = DateTime.Now, TaxPercentage = 21, Supplier = "test supplier", WarehouseItemId = warehouseItem.Id, WarehouseItem = warehouseItem, AvailableQuantity = 10, UnitPriceWithoutTax = 5 };
            var batch2 = new Batch() { Name = "Test Batch name 2", PurchaseDate = DateTime.Now, TaxPercentage = 21, Supplier = "test supplier 2", WarehouseItemId = warehouseItem.Id, WarehouseItem = warehouseItem, AvailableQuantity = 10, UnitPriceWithoutTax = 5 };
            dbContext.Batches.Add(batch1);
            dbContext.Batches.Add(batch2);
            await dbContext.SaveChangesAsync();

            // prepare recipe
            var recipeVM = new RecipeCreateEditViewModel() { ImageUrl = "-", Name = "Test", Note = "Test note", BatchSize = 11, DaysOfCure = 60 , ProductType = ProductType.Soap };
            var recipe = new Recipe(recipeVM, new List<RecipeIngredient>());
            dbContext.Recipes.Add(recipe);
            await dbContext.SaveChangesAsync();

            var recipeIngrediet = new RecipeIngredient() { Quantity = 5, Unit = Models.UnitMeasurement.UnitType.L, Recipe = recipe, RecipeId = recipe.Id, WarehouseItem = warehouseItem, WarehouseItemId = warehouseItem.Id };
            recipe.Ingredients.Add(recipeIngrediet);
            dbContext.RecipeIngredients.Add(recipeIngrediet);
            dbContext.Recipes.Update(recipe);
            await dbContext.SaveChangesAsync();

            var cooking = new Models.Cooking.Cooking() { BatchSize = 10, CookingDate = DateTime.Now, Recipe = recipe, RecipeId = recipe.Id, RecipeNotes = "Old recipe notes", CookingNotes = "Cooking notes", CuringDate = DateTime.Now.AddDays(5), IsCut = false };
            dbContext.Cookings.Add(cooking);
            await dbContext.SaveChangesAsync();
            var beforeCount = await dbContext.Cookings.CountAsync();

            // Act
            cooking.CookingNotes = "Test";

            var result = await controller.DeleteConfirmed(cooking.Id) as RedirectToActionResult;
            var afterCount = await dbContext.Cookings.CountAsync();

            // Assert
            Assert.Equal("Index", result.ActionName);
            Assert.Equal(beforeCount - 1, afterCount);
            Assert.Null(await dbContext.Cookings.FindAsync(recipe.Id));
        }

        [Fact]
        public async Task ExportToPdfTest()
        {
        }


        [Fact]
        public async Task CutConfirm()
        {
        }
    }
}
