using SoapProductionApp.Models.Recipe;

namespace SoapProductionApp.Models.ProductUnit.ViewModels
{
    public class ProductUnitOverviewViewModel
    {
        public int CookingId { get; set; }
        public string RecipeName { get; set; }
        public ProductType ProductType { get; set; }
        public DateTime ExpirationDate { get; set; }
        public decimal Cost { get; set; }
        public int TotalUnits { get; set; }
        public int UnsoldUnits { get; set; }
    }
}
