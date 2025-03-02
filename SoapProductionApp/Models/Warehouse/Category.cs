using System.ComponentModel.DataAnnotations;

namespace SoapProductionApp.Models.Warehouse
{
    public class Category
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string ColorBackground { get; set; }
        public string ColorText { get; set; }

        public virtual List<WarehouseItem> WarehouseItems { get; set; } = new();

        private const string DefaultBackgroundColor = "#ffffff";
        private const string DefaultTextColor = "#000000";

        public Category(string name)
        {
            Name = name;
            ColorBackground = DefaultBackgroundColor;
            ColorText = DefaultTextColor;
        }

        public Category(string name, string? colorBackground, string? colorText)
        {
            Name = name;
            ColorBackground = colorBackground ?? DefaultBackgroundColor;
            ColorText = colorText ?? DefaultTextColor;
        }
    }
}
