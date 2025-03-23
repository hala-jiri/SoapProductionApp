using SoapProductionApp.Models.Recipe;

namespace SoapProductionApp.Models.ProductUnit.ViewModels
{
    public class ProductUnitDetailViewModel
    {
        public int CookingId { get; set; }
        public string RecipeName { get; set; }
        public ProductType ProductType { get; set; }

        public int TotalUnits { get; set; }
        public int SoldUnits { get; set; }
        public int UnsoldUnits { get; set; }

        public List<ProductUnit> Units { get; set; }
    }

}
