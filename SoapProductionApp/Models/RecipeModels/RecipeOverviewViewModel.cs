namespace SoapProductionApp.Models.RecipeModels
{
    public class RecipeOverviewViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int BatchSize { get; set; }
        public decimal TotalCost { get; set; }
        public decimal CostPerPiece { get; set; }
        public bool HasEnoughStock { get; set; }
    }
}
