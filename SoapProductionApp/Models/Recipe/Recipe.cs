using SoapProductionApp.Models.Recipe.ViewModels;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SoapProductionApp.Models.Recipe
{
    public class Recipe
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string ImageUrl { get; set; }

        [Required]
        public int BatchSize { get; set; }

        [Required]
        public int DaysOfCure { get; set; }

        public virtual List<RecipeIngredient> Ingredients { get; set; } = new();

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
        public Recipe(RecipeCreateEditViewModel model, List<RecipeIngredient> ingredients)
        {
            Name = model.Name;
            ImageUrl = model.ImageUrl;
            BatchSize = model.BatchSize;
            DaysOfCure = model.DaysOfCure;
            Note = model.Note ?? string.Empty;
            Ingredients = ingredients;
        }
    }

}
