using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace SoapProductionApp.Models.RecipeModels
{
    public class Recipe
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Name of recipe")]
        public string Name { get; set; }

        [Required]
        [Range(1, 100)]
        [Display(Name = "Batch size (pcs)")]
        public int BatchSize { get; set; }

        public virtual List<RecipeIngredient> Ingredients { get; set; } = new List<RecipeIngredient>();

        [AllowNull]
        [Display(Name = "Picture")]
        public string? ImagePath { get; set; }
    }
}
