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
    public class BatchTest
    {
        private ApplicationDbContext GetDatabaseContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            var dbContext = new ApplicationDbContext(options);
            dbContext.Database.EnsureDeleted(); // Každý test začíná s prázdnou DB
            dbContext.Database.EnsureCreated();
            return dbContext;
        }

        [Fact]
        public async Task AddTwoBatchesAndCalculateTotalVolumeAndAveragePrice()
        {
            // Arrange
            var dbContext = GetDatabaseContext();

            var warehouseItem = new WarehouseItem() { Name = "Test warehouse Item", Unit = Models.UnitMeasurement.UnitType.L, TaxPercentage = 21, MinimumQuantityAlarm = 1, Notes = "Note for test warehouse Item" };
            dbContext.WarehouseItems.Add(warehouseItem);
            await dbContext.SaveChangesAsync();

            var batch1 = new Batch() { Name = "Test Batch name",    PurchaseDate = DateTime.Now, TaxPercentage = 21, Supplier = "test supplier",    WarehouseItemId = warehouseItem.Id,     WarehouseItem = warehouseItem,  AvailableQuantity = 10,     UnitPriceWithoutTax = 5 };
            var batch2 = new Batch() { Name = "Test Batch name 2",  PurchaseDate = DateTime.Now, TaxPercentage = 21, Supplier = "test supplier 2",  WarehouseItemId = warehouseItem.Id,     WarehouseItem = warehouseItem,  AvailableQuantity = 20,     UnitPriceWithoutTax = 5 };
            dbContext.Batches.Add(batch1);
            dbContext.Batches.Add(batch2);
            await dbContext.SaveChangesAsync();


            // Act
            var warehouseItemWithBatches = await dbContext.WarehouseItems.FindAsync(warehouseItem.Id);
            var totalMaterialValueWithoutTax = batch1.PriceOfAvailableStockQuantityWithoutTax + batch2.PriceOfAvailableStockQuantityWithoutTax;
            var totalMaterialValueWithTax = batch1.PriceOfAvailableStockQuantityWithTax + batch2.PriceOfAvailableStockQuantityWithTax;
            var averagePricePerUnitWithoutTax = (batch1.UnitPriceWithoutTax * batch1.AvailableQuantity +
                                                batch2.UnitPriceWithoutTax * batch2.AvailableQuantity) / (batch1.AvailableQuantity + batch2.AvailableQuantity);

            // Assert
            Assert.Equal(30, warehouseItemWithBatches.TotalAvailableQuantity);
            Assert.Equal(totalMaterialValueWithoutTax, warehouseItemWithBatches.TotalMaterialValueWithoutTax);
            Assert.Equal(totalMaterialValueWithTax , warehouseItemWithBatches.TotalMaterialValueWithTax);
            Assert.Equal(averagePricePerUnitWithoutTax, warehouseItemWithBatches.AveragePricePerUnitWithoutTax);

        }
    }
}
