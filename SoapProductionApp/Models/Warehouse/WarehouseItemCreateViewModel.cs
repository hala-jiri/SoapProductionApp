using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace SoapProductionApp.Models.Warehouse
{
    public class WarehouseItemCreateViewModel
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        // Jednotka, ve které se evidence vede (např. L, Kg, Pcs)
        [Required]
        public UnitMeasurement.UnitType Unit { get; set; }

        // NOVĚ: Výchozí jednotka pro položku (např. u olivového oleje může být L, u esenciálních olejů ml)
        [Required]
        public UnitMeasurement.UnitType DefaultUnit { get; set; }

        public List<int> SelectedCategoryIds { get; set; } = new List<int>();
        public List<Category> AvailableCategories { get; set; } = new List<Category>();

        public List<UnitMeasurement.UnitType> AvailableUnits { get; set; } = new List<UnitMeasurement.UnitType>();

        [Required]
        [Range(0, 100, ErrorMessage = "DPH musí být mezi 0 a 100.")]
        public int TaxPercentage { get; set; }

        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Minimální množství musí být kladné.")]
        [DataType(DataType.Text)] // Podporuje desetinná čísla
        public decimal MinQuantity { get; set; }

        public string Supplier { get; set; }
        public string Notes { get; set; }

        // Vypočítané vlastnosti – nebudou zadávány přes formulář
        public decimal Quantity { get; }
        public decimal PricePerUnit { get; }

        // Aktualizovaný konstruktor včetně DefaultUnit a kategorií
        public WarehouseItemCreateViewModel(WarehouseItem warehouseItem, List<Category> allCategories)
        {
            // Základní vlastnosti
            Id = warehouseItem.Id;
            Name = warehouseItem.Name;
            Unit = warehouseItem.Unit;
            DefaultUnit = warehouseItem.DefaultUnit;
            TaxPercentage = warehouseItem.TaxPercentage;
            MinQuantity = warehouseItem.MinQuantity;
            Supplier = warehouseItem.Supplier;
            Notes = warehouseItem.Notes;

            // Přiřazené kategorie (Selected)
            SelectedCategoryIds = warehouseItem.Categories.Select(c => c.Id).ToList();

            // Dostupné kategorie (všechny z databáze)
            AvailableCategories = allCategories;

            // Dostupné jednotky
            AvailableUnits = Enum.GetValues(typeof(UnitMeasurement.UnitType)).Cast<UnitMeasurement.UnitType>().ToList();

            // Vypočítané hodnoty
            Quantity = warehouseItem.TotalQuantityInBaseUnit;
            PricePerUnit = warehouseItem.AveragePricePerBaseUnit;
        }

        // Prázdný konstruktor pro Razor Pages
        public WarehouseItemCreateViewModel() { }
    }
}