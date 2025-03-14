﻿namespace SoapProductionApp.Models
{
    public class UnitMeasurement
    {
        public enum MeasurementCategory
        {
            Volume,   // Objemové jednotky (L, ml)
            Weight,   // Hmotnostní jednotky (Kg, g)
            Count     // Počitatelné jednotky (kusy)
        }

        public enum UnitType
        {
            L = 1,      // Litr
            ml = 2,     // Mililitr
            Kg = 3,     // Kilogram
            g = 4,      // Gram
            Pcs = 5     // Kusy (Pieces)
        }

        public static Dictionary<UnitType, decimal> ConversionFactors = new()
        {
            { UnitType.L, 1000m },  // 1 L = 1000 ml
            { UnitType.ml, 1m },    // 1 ml = 1 ml
            { UnitType.Kg, 1000m }, // 1 kg = 1000 g
            { UnitType.g, 1m },     // 1 g = 1 g
            { UnitType.Pcs, 1m }    // 1 kus = 1 kus
        };

        public static int ConvertToBaseUnit(UnitType unit, decimal quantity)
        {
            if (!ConversionFactors.ContainsKey(unit))
                throw new InvalidOperationException($"Neznámá jednotka {unit}");

            return (int)(quantity * ConversionFactors[unit]);
        }

        public static double ConvertFromBaseUnitToPreferedUnit(UnitType preferedUnit, int quantity)
        {
            if (!ConversionFactors.ContainsKey(preferedUnit))
                throw new InvalidOperationException($"Neznámá jednotka {preferedUnit}");

            return (double)(quantity / ConversionFactors[preferedUnit]);
        }

        public static UnitType GetBaseUnit(UnitType unit)
        {
            return unit switch
            {
                UnitType.L => UnitType.ml,    // Základní jednotka pro L je ml
                UnitType.Kg => UnitType.g,    // Základní jednotka pro Kg je g
                UnitType.ml => UnitType.ml,   // ml je již základní jednotka
                UnitType.g => UnitType.g,     // g je již základní jednotka
                UnitType.Pcs => UnitType.Pcs, // Pcs je již základní jednotka
                _ => throw new ArgumentOutOfRangeException($"Neznámá jednotka {unit}")
            };
        }



        // Statická metoda pro získání kategorie na základě typu jednotky
        public static MeasurementCategory GetCategory(UnitType unit)
        {
            switch (unit)
            {
                case UnitType.L:
                case UnitType.ml:
                    return MeasurementCategory.Volume;
                case UnitType.Kg:
                case UnitType.g:
                    return MeasurementCategory.Weight;
                case UnitType.Pcs:
                    return MeasurementCategory.Count;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public static List<UnitType> GetCompatibleUnits(UnitType unit)
        {
            var category = GetCategory(unit);
            return Enum.GetValues(typeof(UnitType)).Cast<UnitType>()
                .Where(u => GetCategory(u) == category)
                .ToList();
        }
    }
}
