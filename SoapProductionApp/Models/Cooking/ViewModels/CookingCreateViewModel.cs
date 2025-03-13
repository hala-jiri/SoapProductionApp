using SoapProductionApp.Models.Recipe.ViewModels;
using SoapProductionApp.Models.Recipe;

namespace SoapProductionApp.Models.Cooking.ViewModels
{
    public class CookingCreateViewModel
    {
        // Zde si pamatujeme, jaký recept uživatel vybral
        public int? SelectedRecipeId { get; set; }

        // Tyto údaje vyplníme z vybraného receptu
        public int BatchSize { get; set; }
        public string? RecipeNotes { get; set; }
        public string? CookingNotes { get; set; }
        public string? ImageUrl { get; set; }
        public string? ThumbnailUrl { get; set; }

        // V dropdownu nabídneme všechny recepty
        public List<Recipe.Recipe>? Recipes { get; set; }

        // Seznam použitých ingrediencí
        public List<CookingIngredientViewModel> UsedIngredients { get; set; } = new List<CookingIngredientViewModel>();
    }
}
