using System;
using System.Collections.Generic;
using SoapProductionApp.Models.Recipe;

namespace SoapProductionApp.Models.Cooking.ViewModels
{
    public class CookingDetailViewModel
    {
        public int Id { get; set; }
        public string RecipeName { get; set; }
        public int BatchSize { get; set; }
        public DateTime CookingDate { get; set; }
        public DateTime CuringDate { get; set; }
        public decimal TotalCost { get; set; }
        public decimal CostPerSoap { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public string? RecipeNotes { get; set; }
        public string? CookingNotes { get; set; }
        public bool IsCut { get; set; }
        public bool IsReadyToBeSold { get; set; }

        // Seznam použitých ingrediencí
        public List<CookingIngredientViewModel> UsedIngredients { get; set; } = new List<CookingIngredientViewModel>();
    }

    public class CookingIngredientViewModel
    {
        public string IngredientName { get; set; }
        public decimal QuantityUsed { get; set; }
        public string Unit { get; set; }
        public decimal Cost { get; set; }
        public DateTime ExpirationDate { get; set; }
    }
}