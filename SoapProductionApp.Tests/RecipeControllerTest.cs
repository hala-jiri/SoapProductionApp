using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SoapProductionApp.Controllers;
using SoapProductionApp.Data;
using SoapProductionApp.Models.Warehouse.ViewModels;
using SoapProductionApp.Models.Warehouse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SoapProductionApp.Models.Recipe.ViewModels;
using SoapProductionApp.Models.Recipe;
using Moq;
using SoapProductionApp.Services;

namespace SoapProductionApp.Tests
{
    public class RecipeControllerTest
    {
        private ApplicationDbContext GetDatabaseContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "RecipeControllerTestDatabase")
                .Options;

            var dbContext = new ApplicationDbContext(options);
            dbContext.Database.EnsureDeleted(); // Každý test začíná s prázdnou DB
            dbContext.Database.EnsureCreated();
            return dbContext;
        }

        private RecipeController GetController(ApplicationDbContext context)
        {
            var mockAudit = new Mock<IAuditService>();
            return new RecipeController(context, mockAudit.Object);
        }

        [Fact]
        public async Task Create_AddsEnmptyRecipeAndRedirectsToIndex()
        {
            // Arrange
            var dbContext = GetDatabaseContext();
            var controller = GetController(dbContext);

            var recipe = new RecipeCreateEditViewModel()
            {
                ImageUrl = "-",
                Name = "Test",
                Note = "Test note",
                BatchSize = 11,
                DaysOfCure = 60,
                ProductType = ProductType.Soap
            };

            // Act
            var beforeCount = await dbContext.Recipes.CountAsync();
            var result = await controller.Create(recipe) as RedirectToActionResult;
            var afterCount = await dbContext.Recipes.CountAsync();

            // Assert
            Assert.Equal("Index", result.ActionName);
            Assert.Equal(beforeCount + 1, afterCount);
        }

        [Fact]
        public async Task Delete_RemovesRecipeAndRedirectsToIndex()
        {
            // Arrange
            var dbContext = GetDatabaseContext();
            var controller = GetController(dbContext);

            var recipe = new Recipe()
            {
                Name = "Test",
                BatchSize = 11,
                DaysOfCure = 60,
                ProductType = ProductType.Soap,
                ImageUrl = "-",
            };
               
            dbContext.Recipes.Add(recipe);
            await dbContext.SaveChangesAsync();

            var beforeCount = await dbContext.Recipes.CountAsync();

            // Act
            var result = await controller.DeleteConfirmed(recipe.Id) as RedirectToActionResult;
            var afterCount = await dbContext.Recipes.CountAsync();

            // Assert
            Assert.Equal("Index", result.ActionName);
            Assert.Equal(beforeCount - 1, afterCount);
            Assert.Null(await dbContext.Batches.FindAsync(recipe.Id));
        }

        [Fact]
        public async Task Edit_UpdatesRecipeCorrectly()
        {
            // Arrange
            var dbContext = GetDatabaseContext();
            var controller = GetController(dbContext);

            var recipe = new Recipe()
            {
                Name = "Test",
                BatchSize = 11,
                DaysOfCure = 60,
                ProductType = ProductType.Soap,
                ImageUrl = "-",
            };

            dbContext.Recipes.Add(recipe);
            await dbContext.SaveChangesAsync();

            var recipeViewModel = new RecipeCreateEditViewModel()
            {
                Id = recipe.Id,
                Name = "Test 2",
                BatchSize = 20,
                ProductType = ProductType.Soap,
                ImageUrl = "TestUrl",
                DaysOfCure = 30
            };

            // Act
            var result = await controller.Edit(recipeViewModel) as RedirectToActionResult;
            var updatedRecipe = await dbContext.Recipes.FindAsync(recipeViewModel.Id);

            // Assert
            Assert.Equal("Index", result.ActionName);
            Assert.Equal("Test 2", recipeViewModel.Name);
            Assert.Equal(20, recipeViewModel.BatchSize);
            Assert.Equal(30, recipeViewModel.DaysOfCure);
        }
    }
}
