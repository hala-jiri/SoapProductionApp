using SoapProductionApp.Models.Warehouse;
using System.ComponentModel.DataAnnotations;

namespace SoapProductionApp.Models.Recipe.ViewModels
{
    /*public class RecipeOverviewViewModel
    {
        public int Id { get; set; }



        public string ImageUrl { get; set; }

        [Required]
        public int BatchSize { get; set; }

        [Required]
        public int DaysOfCure { get; set; }

        public List<RecipeIngredientViewModel> Ingredients { get; set; } = new List<RecipeIngredientViewModel>();

        // Pro seznam skladových položek, aby si uživatel mohl vybírat ingredience
        public List<RecipeIngredientDetailViewModel> AvailableWarehouseItems { get; set; }

        public string? Note { get; set; }
    }

    public class RecipeIngredientDetailViewModel
    {
        public int WarehouseItemId { get; set; }
        public string WarehouseItemName { get; set; } = string.Empty;

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Quantity must be greater than zero.")]
        [DisplayFormat(DataFormatString = "{0:0.00}", ApplyFormatInEditMode = true)]
        public decimal Quantity { get; set; }

        public string Unit { get; set; } // L, ml, kg, g, ks
        public decimal Cost { get; set; }
    
    }*/
}
