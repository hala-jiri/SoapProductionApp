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

namespace SoapProductionApp.Tests
{
    public class BatchControllerTest
    {
        private ApplicationDbContext GetDatabaseContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "BatchTestDatabase")
                .Options;

            var dbContext = new ApplicationDbContext(options);
            dbContext.Database.EnsureDeleted(); // Každý test začíná s prázdnou DB
            dbContext.Database.EnsureCreated();
            return dbContext;
        }

        [Fact]
        public async Task Create_AddsWarehouseItemBatchAndRedirectsToIndex()
        {
            // Arrange
            var dbContext = GetDatabaseContext();
            var controller = new BatchController(dbContext);

            var warehouseItem = new WarehouseItem()
            {
                Name = "Test warehouse Item",
                Unit = Models.UnitMeasurement.UnitType.L,
                TaxPercentage = 21,
                MinimumQuantityAlarm = 1,
                Notes = "Note for test warehouse Item"
            };
            dbContext.WarehouseItems.Add(warehouseItem);
            await dbContext.SaveChangesAsync();

            var batch = new Batch()
            {
                Name = "Test Batch name",
                PurchaseDate = DateTime.Now,
                TaxPercentage = 21,
                AvailableQuantity = 10,
                Supplier = "test supplier",
                UnitPriceWithoutTax = 5,
                WarehouseItemId = warehouseItem.Id
            };

            var batchCreateViewModel = new BatchCreateViewModel(batch);


            // Act
            var beforeCount = await dbContext.Batches.CountAsync();
            var result = await controller.Create(batchCreateViewModel) as RedirectToActionResult;
            var afterCount = await dbContext.Batches.CountAsync();

            // Assert
            Assert.Equal("Details", result.ActionName);
            Assert.Equal(beforeCount + 1, afterCount);
        }

        [Fact]
        public async Task Delete_RemovesWarehouseItemBatchAndRedirectsToIndex()
        {
            // Arrange
            var dbContext = GetDatabaseContext();
            var controller = new BatchController(dbContext);

            var warehouseItem = new WarehouseItem()
            {
                Name = "Test warehouse Item",
                Unit = Models.UnitMeasurement.UnitType.L,
                TaxPercentage = 21,
                MinimumQuantityAlarm = 1,
                Notes = "Note for test warehouse Item"
            };
            dbContext.WarehouseItems.Add(warehouseItem);
            await dbContext.SaveChangesAsync();

            var batch = new Batch()
            {
                Name = "Test Batch name",
                PurchaseDate = DateTime.Now,
                TaxPercentage = 21,
                AvailableQuantity = 10,
                Supplier = "test supplier",
                UnitPriceWithoutTax = 5,
                WarehouseItemId = warehouseItem.Id,
                WarehouseItem = warehouseItem
            };
            dbContext.Batches.Add(batch);
            await dbContext.SaveChangesAsync();


            var beforeCount = await dbContext.Batches.CountAsync();

            // Act
            var result = await controller.DeleteConfirmed(batch.Id) as RedirectToActionResult;
            var afterCount = await dbContext.Batches.CountAsync();

            // Assert
            Assert.Equal("Details", result.ActionName);
            Assert.Equal(beforeCount - 1, afterCount);
            Assert.Null(await dbContext.Batches.FindAsync(batch.Id));
        }

        [Fact]
        public async Task Edit_UpdatesWarehouseItemBatchCorrectly()
        {
            // Arrange
            var dbContext = GetDatabaseContext();
            var controller = new BatchController(dbContext);

            var warehouseItem = new WarehouseItem()
            {
                Name = "Test warehouse Item",
                Unit = Models.UnitMeasurement.UnitType.L,
                TaxPercentage = 21,
                MinimumQuantityAlarm = 1,
                Notes = "Note for test warehouse Item"
            };
            dbContext.WarehouseItems.Add(warehouseItem);
            await dbContext.SaveChangesAsync();

            var batch = new Batch()
            {
                Name = "Test Batch name",
                PurchaseDate = DateTime.Now,
                TaxPercentage = 21,
                AvailableQuantity = 10,
                Supplier = "test supplier",
                UnitPriceWithoutTax = 5,
                WarehouseItemId = warehouseItem.Id,
                WarehouseItem = warehouseItem
            };
            dbContext.Batches.Add(batch);
            await dbContext.SaveChangesAsync();

            var batchCreateViewModel = new BatchCreateViewModel(batch);

            // Act
            batchCreateViewModel.Name = "New test name";
            batchCreateViewModel.TaxPercentage = 0;
            var result = await controller.Edit(batchCreateViewModel.Id, batchCreateViewModel) as RedirectToActionResult;
            var updatedWarehouseItemBatch = await dbContext.Batches.FindAsync(batchCreateViewModel.Id);

            // Assert
            Assert.Equal("Details", result.ActionName);
            Assert.Equal("New test name", updatedWarehouseItemBatch.Name);
            Assert.Equal(0, updatedWarehouseItemBatch.TaxPercentage);
        }
    }
}
