using SoapProductionApp.Models.Recipe;
using SoapProductionApp.Models.Recipe.ViewModels;
using SoapProductionApp.Models.Warehouse;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace SoapProductionApp.Models.Recipe
{
    public class Recipe
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string ImageUrl { get; set; } // Odkaz na obrázek

        [Required]
        public int BatchSize { get; set; } // Kolik kusů mýdla vznikne

        [Required]
        public int DaysOfCure { get; set; } // Počet dní zrání

        public virtual List<RecipeIngredient> Ingredients { get; set; } = new List<RecipeIngredient>();

        [NotMapped]
        public decimal CostPerBatch => Ingredients.Sum(x => x.CostPerIngredient);

        [NotMapped]
        public decimal CostPerSliceOfBatch => CostPerBatch / BatchSize;

        [NotMapped]
        public bool AreAllIngredientsInStock => Ingredients.All(i => i.IsInStock);

        public string? Note { get; set; }

        public Recipe()
        {
        }

        public Recipe(RecipeCreateEditViewModel model)
        {
            Name = model.Name;
            ImageUrl = model.ImageUrl;
            BatchSize = model.BatchSize;
            DaysOfCure = model.DaysOfCure;
            Note = model.Note;
        }
    }

}
