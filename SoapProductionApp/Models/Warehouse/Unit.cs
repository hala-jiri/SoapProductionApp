using System.ComponentModel.DataAnnotations;

namespace SoapProductionApp.Models.Warehouse
{
    public class Unit
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        // Relace k WarehouseItem (každý item má jednu jednotku)
        public virtual List<WarehouseItem> WarehouseItems { get; set; }
    }
}
