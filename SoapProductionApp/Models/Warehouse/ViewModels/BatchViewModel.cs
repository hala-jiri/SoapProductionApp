using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using SoapProductionApp.Models.Warehouse;

namespace SoapProductionApp.Models.Warehouse.ViewModels
{
    public class BatchViewModel
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
        [Range(0, double.MaxValue)]
        public double QuantityOfPackage { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public double AvailableQuantityOfPackage { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public decimal PriceOfPackageWithoutTax { get; set; }

        [Range(0, double.MaxValue)]
        public decimal PricePerUnitWithoutTax => QuantityOfPackage > 0 ? PriceOfPackageWithoutTax / (decimal)QuantityOfPackage : 0;

        [Range(0, double.MaxValue)]
        public decimal PricePerUnitWithTax => PricePerUnitWithoutTax * (1 + TaxPercentage / 100);

        [Required]
        public UnitMeasurement.UnitType Unit { get; set; }


        [ForeignKey("WarehouseItem")]
        public int WarehouseItemId { get; set; }

        public WarehouseItem WarehouseItem { get; set; }

        public BatchViewModel(Batch batch)
        {
            Id = batch.Id;
            Name = batch.Name;
            Supplier = batch.Supplier;
            PurchaseDate = batch.PurchaseDate;
            ExpirationDate = batch.ExpirationDate;
            TaxPercentage = batch.TaxPercentage;
            Unit = batch.Unit;
            QuantityOfPackage = (double)batch.AvailableQuantity;
            PriceOfPackageWithoutTax = (decimal)QuantityOfPackage * PriceOfPackageWithoutTax;
            WarehouseItem = batch.WarehouseItem;
        }

        public BatchViewModel()
        {
        }
    }
}
