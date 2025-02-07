using System.ComponentModel.DataAnnotations;

namespace SoapProductionApp.Models.Warehouse
{
    public class WarehouseItem
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } // Název položky (např. olivový olej)

        [Required]
        public string Category { get; set; } // Např. "fragrance oils", "packaging"

        [Required]
        public string Unit { get; set; } // Např. "kg", "L", "ks"

        public decimal PricePerUnit { get; set; } // Cena za jednotku bez DPH

        public decimal TaxPercentage { get; set; } // DPH v %

        public decimal PriceWithTax => PricePerUnit * (1 + TaxPercentage / 100); // Cena s DPH

        public decimal Quantity { get; set; } // Celkové množství na skladě

        public decimal MinQuantity { get; set; } // Minimální množství před upozorněním

        public string Supplier { get; set; } // Odkud bylo nakoupeno

        public string Notes { get; set; } // Poznámky

        public virtual List<WarehouseItemBatch> Batches { get; set; } // Historie nákupů a expirace
    }

    public class WarehouseItemBatch
    {
        public int Id { get; set; }

        public int WarehouseItemId { get; set; }
        public virtual WarehouseItem WarehouseItem { get; set; }

        public decimal Quantity { get; set; } // Množství v tomto nákupu

        public decimal PricePerUnit { get; set; } // Cena v tomto nákupu

        public DateTime? ExpirationDate { get; set; } // Datum expirace (může být null)

        public DateTime PurchaseDate { get; set; } = DateTime.Now; // Datum nákupu
    }
}
