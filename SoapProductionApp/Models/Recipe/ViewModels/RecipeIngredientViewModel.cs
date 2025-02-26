namespace SoapProductionApp.Models.Recipe.ViewModels
{
    public class RecipeIngredientViewModel
    {
        public int WarehouseItemId { get; set; }
        public string WarehouseItemName { get; set; }
        public decimal Quantity { get; set; }
        public string Unit { get; set; } // L, ml, kg, g, ks
        public decimal Cost { get; set; }
    }
}
