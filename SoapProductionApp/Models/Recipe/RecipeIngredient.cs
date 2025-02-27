using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SoapProductionApp.Models.Warehouse;


namespace SoapProductionApp.Models.Recipe
{
    public class RecipeIngredient
    {
        public int Id { get; set; }

        [Required]
        public int RecipeId { get; set; }

        [ForeignKey("RecipeId")]
        public Recipe Recipe { get; set; }

        [Required]
        public int WarehouseItemId { get; set; }

        [ForeignKey("WarehouseItemId")]
        public WarehouseItem WarehouseItem { get; set; } // Ingredience pochází ze skladu

        [Required]
        public decimal Quantity { get; set; } // Množství ingredience v receptu

        [Required]
        public UnitMeasurement.UnitType Unit { get; set; } // Typ jednotky (ml, g, ks)

        [NotMapped]
        public decimal CostPerIngredient => WarehouseItem?.AveragePricePerUnitWithoutTax * Quantity ?? 0;

        [NotMapped]
        public bool IsInStock => WarehouseItem?.TotalAvailableQuantity >= Quantity;

        [NotMapped]
        public DateTime? NearestExpirationDate =>
            WarehouseItem?.Batches?
                .Where(b => b.AvailableQuantity > 0)
                .OrderBy(b => b.ExpirationDate)
                .Select(b => b.ExpirationDate)
                .FirstOrDefault();
    }
}
