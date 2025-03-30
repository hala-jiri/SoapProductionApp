using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using SoapProductionApp.Controllers;
using SoapProductionApp.Data;
using SoapProductionApp.Models.Warehouse;
using SoapProductionApp.Models.Warehouse.ViewModels;
using SoapProductionApp.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoapProductionApp.Tests
{
    public class WarehouseControllerTest
    {
        private ApplicationDbContext GetDatabaseContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "WarehouseItemTestDatabase")
                .Options;

            var dbContext = new ApplicationDbContext(options);
            dbContext.Database.EnsureDeleted(); // Každý test začíná s prázdnou DB
            dbContext.Database.EnsureCreated();
            return dbContext;
        }

        private WarehouseController GetController(ApplicationDbContext context)
        {
            var mockAudit = new Mock<IAuditService>();
            return new WarehouseController(context, mockAudit.Object);
        }

        [Fact]
        public async Task Create_AddsWarehouseItemWithoutBatchAndRedirectsToIndex()
        {
            // Arrange
            var dbContext = GetDatabaseContext();
            var controller = GetController(dbContext);
            var warehouseItemCreateEditViewModel = new WarehouseItemCreateEditViewModel() {
                Name = "Test warehouse Item",
                Unit = Models.UnitMeasurement.UnitType.L,
                TaxPercentage = 21,
                MinimumQuantityAlarm = 1,
                Notes = "Note for test warehouse Item"
            };

            // Act
            var beforeCount = dbContext.WarehouseItems.Count();
            var result = await controller.Create(warehouseItemCreateEditViewModel) as RedirectToActionResult;
            var afterCount = dbContext.WarehouseItems.Count();

            // Assert
            Assert.Equal("Index", result.ActionName);
            Assert.Equal(beforeCount + 1, afterCount);
        }

        [Fact]
        public async Task Delete_RemovesWarehouseItemWithoutBatchAndRedirectsToIndex()
        {
            // Arrange
            var dbContext = GetDatabaseContext();
            var controller = GetController(dbContext);
            var warehouseItem = new WarehouseItem() {
                Name = "Test warehouse Item",
                Unit = Models.UnitMeasurement.UnitType.L
            };
            dbContext.WarehouseItems.Add(warehouseItem);
            await dbContext.SaveChangesAsync();

            var beforeCount = await dbContext.WarehouseItems.CountAsync();

            // Act
            var result = await controller.DeleteConfirmed(warehouseItem.Id) as RedirectToActionResult;
            var afterCount = await dbContext.WarehouseItems.CountAsync();

            // Assert
            Assert.Equal("Index", result.ActionName);
            Assert.Equal(beforeCount - 1, afterCount);
            Assert.Null(await dbContext.WarehouseItems.FindAsync(warehouseItem.Id));
        }

        [Fact]
        public async Task Edit_UpdatesWarehouseItemCorrectly()
        {
            // Arrange
            var dbContext = GetDatabaseContext();
            var controller = GetController(dbContext);

            var warehouseItem = new WarehouseItem()
            {
                Name = "Test warehouse Item",
                Unit = Models.UnitMeasurement.UnitType.L
            }; 
            dbContext.WarehouseItems.Add(warehouseItem);
            dbContext.SaveChanges();

            var warehouseItemCreateEditViewModel = new WarehouseItemCreateEditViewModel(warehouseItem);
            // Act
            warehouseItemCreateEditViewModel.Name = "New test name";
            warehouseItemCreateEditViewModel.Notes = "New test note";
            var result = await controller.Edit(warehouseItemCreateEditViewModel.Id, warehouseItemCreateEditViewModel) as RedirectToActionResult;

            var updatedWarehouseItem = await dbContext.WarehouseItems.FindAsync(warehouseItemCreateEditViewModel.Id);

            // Assert
            Assert.Equal("Index", result.ActionName);
            Assert.Equal("New test name", updatedWarehouseItem.Name);
            Assert.Equal("New test note", updatedWarehouseItem.Notes);
        }
    }
}
