using SoapProductionApp.Models.Recipe;

namespace SoapProductionApp.Models.ProductUnit.ViewModels
{
    public class ProductBatchSummaryViewModel
    {
        public string RecipeName { get; set; }
        public ProductType ProductType { get; set; }
        public DateTime ExpirationDate { get; set; }
        public decimal CostPerUnit { get; set; }
        public int UnitsInStock { get; set; }
        public int TotalUnits { get; set; }
    }
}
