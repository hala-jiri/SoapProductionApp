using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SoapProductionApp.Models.Warehouse
{
    public class Batch
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Supplier { get; set; }

        [Required]
        public DateTime PurchaseDate { get; set; }

        public DateTime? ExpirationDate { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public double Quantity { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public decimal PricePerUnit { get; set; }

        [ForeignKey("WarehouseItem")]
        public int WarehouseItemId { get; set; }

        [NotMapped] // ⬅ Tohle říká EF, že nemá být součástí databáze ani validace
        public WarehouseItem WarehouseItem { get; set; }
    }
}
