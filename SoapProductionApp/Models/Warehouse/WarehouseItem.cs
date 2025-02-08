using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SoapProductionApp.Models.Warehouse
{
    public class WarehouseItem
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } // Název položky (např. olivový olej)

        public decimal PricePerUnit { get; set; } // Cena za jednotku bez DPH

        public decimal TaxPercentage { get; set; } // DPH v %

        public decimal PriceWithTax => PricePerUnit * (1 + TaxPercentage / 100); // Cena s DPH

        public decimal Quantity { get; set; } // Celkové množství na skladě

        public decimal MinQuantity { get; set; } // Minimální množství před upozorněním

        public string Supplier { get; set; } // Odkud bylo nakoupeno

        public string Notes { get; set; } // Poznámky

        [Required]
        public int UnitId { get; set; }  // Povinný odkaz na Unit
        public virtual Unit Unit { get; set; }  // Relace na Unit

        // Relace na kategorie (položka může mít více kategorií)
        public virtual List<Category> Categories { get; set; }

        public virtual List<WarehouseItemBatch> Batches { get; set; } // Historie nákupů a expirace
    }
}
