using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SoapProductionApp.Models.Warehouse
{
    public class WarehouseItem
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public decimal Quantity => (decimal)(Batches?.Sum(b => b.Quantity) ?? 0); // Celkové množství 

        // Průměrná cena za jednotku = celková hodnota všech batchů děleno celkovým množstvím
        [NotMapped]
        public decimal PricePerUnit
        {
            get
            {
                if (Batches == null || !Batches.Any()) return 0;

                decimal totalQuantity = (decimal)Batches.Sum(b => b.Quantity);
                decimal totalPrice = (decimal)Batches.Sum(b => (decimal)b.Quantity * b.PricePerUnit);

                return totalQuantity > 0 ? totalPrice / totalQuantity : 0;
            }
        }

        public decimal TaxPercentage { get; set; }

        public decimal PriceWithTax => PricePerUnit * (1 + TaxPercentage / 100);

        public decimal MinQuantity { get; set; }

        public string Supplier { get; set; }

        public string Notes { get; set; }

        [Required]
        [ForeignKey("Unit")]
        public int UnitId { get; set; }
        public virtual Unit Unit { get; set; }

        public virtual List<Category> Categories { get; set; } = new List<Category>();

        public virtual List<Batch> Batches { get; set; } = new List<Batch>();
    }
}
