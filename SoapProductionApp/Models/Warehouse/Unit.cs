using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SoapProductionApp.Models.Warehouse
{
    public class Unit
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        // Relace k WarehouseItem (každý item má jednu jednotku)
        //[NotMapped]
        //public virtual List<WarehouseItem> WarehouseItems { get; set; }
    }
}
