using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using SoapProductionApp.Models;

namespace SoapProductionApp.Models.Warehouse
{
    public class WarehouseItem
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        // Výchozí jednotka (např. L nebo Kg)
        [Required]
        public UnitMeasurement.UnitType Unit { get; set; }

        public int TaxPercentage { get; set; }

        // Celkové množství v základních jednotkách (např. ml, g)
        [NotMapped]
        public decimal TotalAvailableQuantity =>
            Batches?.Sum(batch => batch.AvailableQuantity) ?? 0;

        // Prumerna cena za jednotku, could be like "TotalMaterialValueWithoutTax / TotalAvailableQuantity)"
        [NotMapped]
        public decimal AveragePricePerUnitWithoutTax => TotalAvailableQuantity > 0
        ? (Batches?.Sum(batch => batch.UnitPriceWithoutTax * batch.AvailableQuantity) ?? 0) / TotalAvailableQuantity : 0;

        [NotMapped]
        public decimal AveragePricePerUnitWithTax => TotalAvailableQuantity > 0
        ? (Batches?.Sum(batch => batch.UnitPriceWithTax * batch.AvailableQuantity) ?? 0) / TotalAvailableQuantity : 0;


        // Celková hodnota materiálu (bez DPH)
        [NotMapped]
        public decimal TotalMaterialValueWithoutTax => Batches?.Sum(batch => batch.PriceOfAvailableStockQuantityWithoutTax) ?? 0;
        [NotMapped]
        public decimal TotalMaterialValueWithTax => Batches?.Sum(batch => batch.PriceOfAvailableStockQuantityWithTax) ?? 0;


        // Minimální množství ve výchozí jednotce (např. L, Kg)
        public decimal MinimumQuantityAlarm { get; set; }

        [NotMapped]
        public string Suppliers => string.Join(", ", Batches?
            .Select(b => b.Supplier)
            .Where(s => !string.IsNullOrEmpty(s))
            .Distinct() ?? new List<string>());

        public string Notes { get; set; }

        public virtual List<Category> Categories { get; set; }

        public virtual List<Batch> Batches { get; set; } = new List<Batch>();
    }
}