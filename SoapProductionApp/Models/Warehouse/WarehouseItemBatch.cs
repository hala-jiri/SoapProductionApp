using System.ComponentModel.DataAnnotations;

namespace SoapProductionApp.Models.Warehouse
{
    public class WarehouseItemBatch
    {
        public int Id { get; set; }

        [Required]
        public int WarehouseItemId { get; set; }
        public virtual WarehouseItem WarehouseItem { get; set; }

        [Required]
        public decimal Quantity { get; set; } // Kolik bylo zakoupeno

        [Required]
        public decimal PricePerUnit { get; set; } // Cena v této objednávce

        public DateTime? ExpirationDate { get; set; } // Expirace (nepovinná)

        [Required]
        public DateTime PurchaseDate { get; set; } = DateTime.Now; // Datum nákupu
    }
}
