using SoapProductionApp.Models.Recipe;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace SoapProductionApp.Models.ProductUnit
{
    public class ProductUnit
    {
        public int Id { get; set; }

        [ForeignKey("Cooking")]
        public int CookingId { get; set; }
        public Cooking.Cooking Cooking { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public bool IsSold { get; set; } = false;

        public decimal Cost { get; set; }

        public DateTime ExpirationDate { get; set; }

        public ProductType ProductType { get; set; }
    }
}
