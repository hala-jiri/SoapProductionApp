using SoapProductionApp.Models.Warehouse;
using System.ComponentModel.DataAnnotations;

namespace SoapProductionApp.Models.Recipe.ViewModels
{
    public class RecipeCreateEditViewModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string ImageUrl { get; set; }

        [Required]
        public int BatchSize { get; set; }

        [Required]
        public int DaysOfCure { get; set; }

        public List<RecipeIngredientViewModel> Ingredients { get; set; } = new List<RecipeIngredientViewModel>();

        // Pro seznam skladových položek, aby si uživatel mohl vybírat ingredience
        public List<WarehouseItem> AvailableWarehouseItems { get; set; }
    }
}
