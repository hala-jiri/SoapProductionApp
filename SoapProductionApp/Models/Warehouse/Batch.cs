using SoapProductionApp.Models.Warehouse.ViewModels;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace SoapProductionApp.Models.Warehouse
{
    public class Batch
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Supplier { get; set; }
        [Required]
        public DateTime PurchaseDate { get; set; }
        public DateTime? ExpirationDate { get; set; }



        [Required]
        public int TaxPercentage { get; set; }

        [Required]
        public decimal AvailableQuantity { get; set; }

        [NotMapped]
        public decimal PriceOfAvailableStockQuantityWithoutTax => AvailableQuantity * UnitPriceWithoutTax;

        [NotMapped]
        public decimal PriceOfAvailableStockQuantityWithTax => PriceOfAvailableStockQuantityWithoutTax * (1 + (decimal)TaxPercentage / 100);

        [Required]
        public decimal UnitPriceWithoutTax { get; set; }
        [NotMapped]
        public decimal UnitPriceWithTax => UnitPriceWithoutTax * (1 + (decimal)TaxPercentage / 100);

        [NotMapped]
        public UnitMeasurement.UnitType Unit => WarehouseItem?.Unit ?? UnitMeasurement.UnitType.g;

        [ForeignKey("WarehouseItem")]
        public int WarehouseItemId { get; set; }

        [JsonIgnore] // Zabrání serializaci cyklické reference
        public WarehouseItem WarehouseItem { get; set; }

        public Batch()
        {
        }

        public Batch(BatchCreateViewModel batchCreateViewModel)
        {
            Name = batchCreateViewModel.Name;
            Supplier = batchCreateViewModel.Supplier;
            PurchaseDate = batchCreateViewModel.PurchaseDate;
            ExpirationDate = batchCreateViewModel.ExpirationDate;
            TaxPercentage = batchCreateViewModel.TaxPercentage;

            AvailableQuantity = (decimal)batchCreateViewModel.QuantityOfPackage;
            UnitPriceWithoutTax = batchCreateViewModel.PriceOfPackageWithoutTax / (decimal)batchCreateViewModel.QuantityOfPackage;

            WarehouseItemId = batchCreateViewModel.WarehouseItemId;
        }

    }
}