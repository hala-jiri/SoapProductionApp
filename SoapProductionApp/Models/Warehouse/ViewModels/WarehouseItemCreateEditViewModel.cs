using Microsoft.Extensions.FileSystemGlobbing;
using NuGet.Packaging;
using SoapProductionApp.Models.Warehouse;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace SoapProductionApp.Models.Warehouse.ViewModels
{
    public class WarehouseItemCreateEditViewModel
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        // Jednotka, ve které se evidence vede (např. L, Kg, Pcs)
        public List<UnitMeasurement.UnitType> AvailableUnits { get; set; } = new List<UnitMeasurement.UnitType>();
        // Jednotka, ve které se evidence vede (např. L, Kg, Pcs)
        public UnitMeasurement.UnitType Unit { get; set; }

        public List<int> SelectedCategoryIds { get; set; } = new List<int>();
        public List<Category> AvailableCategories { get; set; } = new List<Category>();

        [Required]
        [Range(0, 100, ErrorMessage = "DPH musí být mezi 0 a 100.")]
        public int TaxPercentage { get; set; }

        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Minimální množství musí být kladné.")]
        [DataType(DataType.Text)] // Podporuje desetinná čísla
        public decimal MinimumQuantityAlarm { get; set; }
        public string Notes { get; set; }


        public WarehouseItemCreateEditViewModel()
        {
        }
    }
}
