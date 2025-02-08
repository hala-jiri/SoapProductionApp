using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace SoapProductionApp.Models.Warehouse
{
    public class WarehouseItemCreateViewModel
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public decimal Quantity { get; set; }

        [Required]
        public int SelectedUnitId { get; set; }

        public List<int> SelectedCategoryIds { get; set; } = new List<int>(); // Zajištění, že nebude null

        public List<Category> AvailableCategories { get; set; } = new List<Category>();
        public List<Unit> AvailableUnits { get; set; } = new List<Unit>();

        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Cena musí být kladná.")]
        public decimal PricePerUnit { get; set; } // Cena za jednotku

        [Required]
        [Range(0, 100, ErrorMessage = "DPH musí být mezi 0 a 100.")]
        public decimal TaxPercentage { get; set; } // DPH v %

        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Minimální množství musí být kladné.")]
        public decimal MinQuantity { get; set; } // Minimální množství pro upozornění

        public string Supplier { get; set; } // Dodavatel
        public string Notes { get; set; } // Poznámky
    }
}
