using System.ComponentModel.DataAnnotations;

namespace SoapProductionApp.Models.Recipe.ViewModels
{
    public class RecipeIngredientViewModel
    {
        public int WarehouseItemId { get; set; }
        
        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Quantity must be greater than zero.")]
        [DisplayFormat(DataFormatString = "{0:0.00}", ApplyFormatInEditMode = true)]
        public decimal Quantity { get; set; }

        public string Unit { get; set; } // L, ml, kg, g, ks
        public decimal Cost { get; set; }
    }
}
