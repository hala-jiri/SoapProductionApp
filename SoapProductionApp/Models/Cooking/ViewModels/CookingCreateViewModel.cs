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

        // V dropdownu nabídneme všechny recepty
        public List<Recipe.Recipe>? Recipes { get; set; }

        // Sem si můžeme uložit ingredience, abychom je zobrazili
        public List<RecipeIngredientViewModel>? Ingredients { get; set; }
    }
}
