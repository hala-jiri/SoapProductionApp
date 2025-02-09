using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SoapProductionApp.Models.Warehouse
{
    public class WarehouseItem
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public decimal Quantity => Batches?.Sum(b => b.Quantity) ?? 0; // Celkové množství 

        public decimal PricePerUnit
        {
            get
            {
                if (Batches == null || !Batches.Any()) return 0;
                return Batches.Sum(b => b.Quantity * b.PricePerUnit) / (Batches.Sum(b => b.Quantity) == 0 ? 1 : Batches.Sum(b => b.Quantity));
            }
        }

        public decimal TaxPercentage { get; set; }

        public decimal PriceWithTax => PricePerUnit * (1 + TaxPercentage / 100);

        public decimal MinQuantity { get; set; }

        public string Supplier { get; set; }

        public string Notes { get; set; }

        [Required]
        public int UnitId { get; set; }
        public virtual Unit Unit { get; set; }

        public virtual List<Category> Categories { get; set; }

        public virtual List<WarehouseItemBatch> Batches { get; set; } = new List<WarehouseItemBatch>();
    }
}
