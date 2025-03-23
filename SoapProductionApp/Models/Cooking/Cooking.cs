using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using SoapProductionApp.Models.ProductUnit;  

namespace SoapProductionApp.Models.Cooking
{ 
    public class Cooking
    {
        public int Id { get; set; }

        [ForeignKey("Recipe")]
        public int RecipeId { get; set; }

        [JsonIgnore] // Zabrání serializaci cyklické reference
        public Recipe.Recipe Recipe { get; set; }

        [Required]
        public int BatchSize { get; set; }

        public bool BatchSizeWasChanged { get; set; }

        public DateTime CookingDate { get; set; }

        public DateTime CuringDate { get; set; }

        [NotMapped]
        public decimal TotalCost => UsedIngredients.Sum(x => x.Cost);

        [NotMapped]
        public decimal CostPerSoap => BatchSize > 0 ? TotalCost / (decimal)BatchSize : 0;

        [NotMapped]
        public DateTime? ExpirationDate => 
            UsedIngredients.Any()
            ? UsedIngredients.Min(i => i.ExpirationDate)
            : (DateTime?)null; // Pokud nejsou ingredience, vrátí null

        public string? RecipeNotes { get; set; }

        public string? CookingNotes { get; set; }

        public bool IsCut { get; set; } = false;

        [NotMapped]
        public bool IsReadyToBeSold => CuringDate <= DateTime.Today;

        public List<CookingIngredient> UsedIngredients { get; set; } = new List<CookingIngredient>();

        public List<ProductUnit.ProductUnit> ProductUnits { get; set; } = new();

        [NotMapped]
        public int TotalUnits => ProductUnits.Count;

        [NotMapped]
        public int UnsoldUnits => ProductUnits.Count(p => !p.IsSold);

        public void CutIntoUnits()
        {
            if (IsCut || ProductUnits.Any())
                return;

            for (int i = 0; i < BatchSize; i++)
            {
                ProductUnits.Add(new ProductUnit.ProductUnit
                {
                    CookingId = this.Id,
                    Cost = this.CostPerSoap,
                    ExpirationDate = this.ExpirationDate ?? this.CuringDate.AddMonths(12),
                    ProductType = this.Recipe.ProductType
                });
            }

            this.IsCut = true;
        }
    }
}
