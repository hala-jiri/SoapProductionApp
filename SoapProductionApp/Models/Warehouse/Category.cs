using System.ComponentModel.DataAnnotations;

namespace SoapProductionApp.Models.Warehouse
{
    public class Category
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string ColorBackground { get; set; } = "#ffffff"; // Výchozí barva pozadí (bílá)
        public string ColorText { get; set; } = "#000000"; // Výchozí barva textu (černá)

        public virtual List<WarehouseItem> WarehouseItems { get; set; }

    }
}
