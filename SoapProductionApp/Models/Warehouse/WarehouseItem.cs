using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using SoapProductionApp.Models;

namespace SoapProductionApp.Models.Warehouse
{
    public class WarehouseItem
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public UnitMeasurement.UnitType DefaultUnit { get; set; }

        [NotMapped]
        public decimal TotalQuantityInBaseUnit
        {
            get
            {
                return Batches?.Sum(batch => batch.QuantityInBaseUnit) ?? 0;
            }
        }

        [NotMapped]
        public decimal AveragePricePerBaseUnit
        {
            get
            {
                if (Batches == null || !Batches.Any()) return 0;
                decimal totalQuantity = TotalQuantityInBaseUnit;
                decimal totalValue = Batches.Sum(batch => batch.QuantityInBaseUnit * batch.PricePerBaseUnit);
                return totalQuantity > 0 ? totalValue / totalQuantity : 0;
            }
        }

        [NotMapped]
        public decimal PriceWithTax => AveragePricePerBaseUnit * (1 + TaxPercentage / 100);


        [NotMapped]
        public decimal TotalMaterialCost => TotalQuantityInBaseUnit * AveragePricePerBaseUnit;

        public decimal TaxPercentage { get; set; }
        public decimal MinQuantity { get; set; }
        public string Supplier { get; set; }
        public string Notes { get; set; }

        [Required]
        public UnitMeasurement.UnitType Unit { get; set; }

        [NotMapped]
        public UnitMeasurement.MeasurementCategory MeasurementCategory => UnitMeasurement.GetCategory(Unit);

        public virtual List<Category> Categories { get; set; } = new List<Category>();

        public virtual List<Batch> Batches { get; set; } = new List<Batch>();

        public decimal ConvertToUnit(UnitMeasurement.UnitType targetUnit, decimal quantity)
        {
            if (MeasurementCategory != UnitMeasurement.GetCategory(targetUnit))
                throw new InvalidOperationException("Cannot convert between units of different categories");
            return UnitMeasurement.ConvertToBaseUnit(targetUnit, quantity) / UnitMeasurement.ConvertToBaseUnit(Unit, 1);
        }

        public List<UnitMeasurement.UnitType> GetAllowedUnits()
        {
            return UnitMeasurement.GetCompatibleUnits(Unit);
        }
    }
}