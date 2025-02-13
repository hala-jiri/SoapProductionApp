using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SoapProductionApp.Models.Warehouse
{
    public class Batch
    {
        public int Id { get; set; }

        public string Name { get; set; }
        public string Supplier { get; set; }

        [Required]
        public DateTime PurchaseDate { get; set; }

        public DateTime? ExpirationDate { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public double QuantityOfPackage { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public decimal PriceOfPackage { get; set; }

        [Required]
        public UnitMeasurement.UnitType Unit { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public decimal QuantityInBaseUnit { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public decimal PricePerBaseUnit { get; set; }

        [Required]
        public decimal TaxPercentage { get; set; }

        [Range(0, double.MaxValue)]
        public decimal AvailableQuantity { get; set; }

        [NotMapped]
        public decimal PricePerBaseUnitWithTax => PricePerBaseUnit * (1 + TaxPercentage / 100);

        [ForeignKey("WarehouseItem")]
        public int WarehouseItemId { get; set; }

        public WarehouseItem WarehouseItem { get; set; }

        public Batch()
        {
            AvailableQuantity = QuantityInBaseUnit;
        }
    }
}