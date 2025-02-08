using System.ComponentModel.DataAnnotations;

namespace SoapProductionApp.Models.Warehouse
{
    public class Category
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        // Relace k WarehouseItem (může mít více kategorií)
        public virtual List<WarehouseItem> WarehouseItems { get; set; }
    }
}
