using SoapProductionApp.Models.Warehouse;
using System.ComponentModel.DataAnnotations;

namespace SoapProductionApp.Models.RecipeModels
{
    public class RecipeIngredient
    {
        public int Id { get; set; }

        [Required]
        public int RecipeId { get; set; }
        public virtual Recipe Recipe { get; set; }

        [Required]
        [Display(Name = "Ingredient")]
        public int WarehouseItemId { get; set; }
        public virtual WarehouseItem WarehouseItem { get; set; }

        [Required]
        [Range(0.01, 1000)]
        [Display(Name = "Quantity")]
        public decimal Quantity { get; set; }

        [Display(Name = "Unit")]
        public string Unit => WarehouseItem?.Unit?.Name;

        [Display(Name = "Price")]
        public decimal Cost => Quantity * WarehouseItem?.PricePerUnit ?? 0;
    }
}
