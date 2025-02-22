using SoapProductionApp.Models.Warehouse;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SoapProductionApp.Models.ViewModels
{
    public class BatchCreateViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Supplier { get; set; }

        [Required]
        public DateTime PurchaseDate { get; set; }
        public DateTime? ExpirationDate { get; set; }

        [Required]
        public int TaxPercentage { get; set; }

        [Required]
        public decimal QuantityOfPackage { get; set; }

        [Required]
        public decimal PriceOfPackageWithoutTax { get; set; }

        [Required]
        public UnitMeasurement.UnitType Unit { get; set; }


        [ForeignKey("WarehouseItem")]
        public int WarehouseItemId { get; set; }
        public WarehouseItem WarehouseItem { get; set; }

        public BatchCreateViewModel()
        {
        }

        public BatchCreateViewModel(Batch batch)
        {
            Id = batch.Id;
            Name = batch.Name;
            Supplier = batch.Supplier;

            PurchaseDate = batch.PurchaseDate;
            ExpirationDate = batch.ExpirationDate;
            TaxPercentage = batch.TaxPercentage;

            QuantityOfPackage = batch.AvailableQuantity;
            PriceOfPackageWithoutTax = batch.PriceOfAvailableStockQuantityWithoutTax;

            Unit = batch.Unit;
            WarehouseItemId = batch.WarehouseItemId;
            WarehouseItem = batch.WarehouseItem;
        }
    }
}
