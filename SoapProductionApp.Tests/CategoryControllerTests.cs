using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SoapProductionApp.Controllers;
using SoapProductionApp.Data;
using SoapProductionApp.Models.Warehouse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoapProductionApp.Tests
{
    public class CategoryControllerTests
    {
        private ApplicationDbContext GetDatabaseContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "CategoryTestDatabase")
                .Options;

            var dbContext = new ApplicationDbContext(options);
            dbContext.Database.EnsureDeleted(); // Každý test začíná s prázdnou DB
            dbContext.Database.EnsureCreated();
            return dbContext;
        }

        [Fact]
        public void Create_AddsCategoryAndRedirectsToIndex()
        {
            // Arrange
            var dbContext = GetDatabaseContext();
            var controller = new CategoryController(dbContext);
            var category = new Category("Test Category") { ColorBackground = "#FFF", ColorText = "#000" };

            // Act
            var beforeCount = dbContext.Categories.Count();
            var result = controller.Create(category) as RedirectToActionResult;
            var afterCount = dbContext.Categories.Count();

            // Assert
            Assert.Equal("Index", result.ActionName);
            Assert.Equal(beforeCount + 1, afterCount);
        }

        [Fact]
        public void Delete_RemovesCategoryFromDatabase()
        {
            // Arrange
            var dbContext = GetDatabaseContext();
            var controller = new CategoryController(dbContext);

            var category = new Category("To Delete") { ColorBackground = "#FFF", ColorText = "#000" };
            dbContext.Categories.Add(category);
            dbContext.SaveChanges();

            var beforeCount = dbContext.Categories.Count();

            // Act
            var result = controller.DeleteConfirmed(category.Id) as RedirectToActionResult;
            var afterCount = dbContext.Categories.Count();

            // Assert
            Assert.Equal("Index", result.ActionName);
            Assert.Equal(beforeCount - 1, afterCount);
            Assert.Null(dbContext.Categories.Find(category.Id)); // Ověříme, že neexistuje
        }

        [Fact]
        public async Task Edit_UpdatesCategoryCorrectly()
        {
            // Arrange
            var dbContext = GetDatabaseContext();
            var controller = new CategoryController(dbContext);

            var category = new Category("Old Name") { ColorBackground = "#FFF", ColorText = "#000" };
            dbContext.Categories.Add(category);
            dbContext.SaveChanges();

            // Act
            category.Name = "New Name";
            category.ColorBackground = "#000";
            var result = await controller.Edit(category.Id, category) as RedirectToActionResult;

            var updatedCategory = await dbContext.Categories.FindAsync(category.Id);

            // Assert
            Assert.Equal("Index", result.ActionName);
            Assert.Equal("New Name", updatedCategory.Name);
            Assert.Equal("#000", updatedCategory.ColorBackground);
        }

    }
}
