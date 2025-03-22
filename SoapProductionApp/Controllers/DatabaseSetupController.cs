using Microsoft.AspNetCore.Mvc;
using SoapProductionApp.Data;
using SoapProductionApp.Models.Recipe;
using SoapProductionApp.Models.Warehouse;
using SoapProductionApp.Models;
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;

namespace SoapProductionApp.Controllers
{
    public class DatabaseSetupController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DatabaseSetupController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ResetDatabase()
        {
            // Smazání všech existujících dat
            _context.Batches.RemoveRange(_context.Batches);
            _context.Recipes.RemoveRange(_context.Recipes);
            _context.WarehouseItems.RemoveRange(_context.WarehouseItems);
            _context.Categories.RemoveRange(_context.Categories);
            await _context.SaveChangesAsync();
            DeleteImagesInUploadFolder();

            // Vytvoření kategorií
            var categoryData = new Dictionary<string, (string Background, string Text)>
            {
                { "Oils", ("#FFCC00", "#000000") },
                { "Essential Oils", ("#00CC99", "#FFFFFF") },
                { "Lye & Additives", ("#1E1E1E", "#FFFFFF") },
                { "Colorants & Pigments", ("#0044CC", "#333333") },
                { "Fragrances", ("#F5F5F5", "#FFFFFF") },
                { "Herbs & Botanicals", ("#800000", "#FFFFFF") },
                { "Molds & Tools", ("#FFCC00", "#FFFFFF") },
                { "Clays", ("#0044CC", "#333333") },
                { "Packaging & Labels", ("#228B22", "#FFFFFF") }
            };

            var categories = categoryData.Select(c => new Category(c.Key)
            {
                ColorBackground = c.Value.Background,
                ColorText = c.Value.Text
            }).ToList();


            _context.Categories.AddRange(categories);
            await _context.SaveChangesAsync();

            var categoryLookup = _context.Categories.ToDictionary(c => c.Name, c => c);

            // Vytvoření skladových položek
            var warehouseItems = new List<WarehouseItem>
            {
                new WarehouseItem { Name = "Sodium Hydroxide",          Unit = UnitMeasurement.UnitType.Kg, TaxPercentage = 21, MinimumQuantityAlarm = 0.5m, Categories = new List<Category> { categoryLookup["Lye & Additives"] } },
                new WarehouseItem { Name = "Distilled water",           Unit = UnitMeasurement.UnitType.L, TaxPercentage = 21,  MinimumQuantityAlarm = 1,   Categories = new List<Category> { categoryLookup["Lye & Additives"] } },
                new WarehouseItem { Name = "Coconut Oil Butter",        Unit = UnitMeasurement.UnitType.Kg, TaxPercentage = 21, MinimumQuantityAlarm = 0.5m, Categories = new List<Category> { categoryLookup["Oils"] } },
                new WarehouseItem { Name = "Shea Butter Organic",       Unit = UnitMeasurement.UnitType.Kg, TaxPercentage = 21, MinimumQuantityAlarm = 0.5m, Categories = new List<Category> { categoryLookup["Oils"] } },
                new WarehouseItem { Name = "Olive Oil Extra Virgin",    Unit = UnitMeasurement.UnitType.L, TaxPercentage = 21,  MinimumQuantityAlarm = 1,   Categories = new List<Category> { categoryLookup["Oils"] } },
                new WarehouseItem { Name = "Castor Oil Cold Pressed",   Unit = UnitMeasurement.UnitType.ml, TaxPercentage = 21, MinimumQuantityAlarm = 500, Categories = new List<Category> { categoryLookup["Oils"] } },
                new WarehouseItem { Name = "Sweet Almond Oil",          Unit = UnitMeasurement.UnitType.ml, TaxPercentage = 21, MinimumQuantityAlarm = 100, Categories = new List<Category> { categoryLookup["Oils"] } },
                new WarehouseItem { Name = "Jojoba Oil",                Unit = UnitMeasurement.UnitType.ml, TaxPercentage = 21, MinimumQuantityAlarm = 100, Categories = new List<Category> { categoryLookup["Oils"] } },
                new WarehouseItem { Name = "Argan Oil",                 Unit = UnitMeasurement.UnitType.ml, TaxPercentage = 21, MinimumQuantityAlarm = 100, Categories = new List<Category> { categoryLookup["Oils"] } },
                new WarehouseItem { Name = "Jasmine Essential Oil Blend",       Unit = UnitMeasurement.UnitType.ml, TaxPercentage = 21, MinimumQuantityAlarm = 15, Categories = new List<Category> { categoryLookup["Essential Oils"] } },
                new WarehouseItem { Name = "Bergamot Essential Oil",            Unit = UnitMeasurement.UnitType.ml, TaxPercentage = 21, MinimumQuantityAlarm = 15, Categories = new List<Category> { categoryLookup["Essential Oils"] } },
                new WarehouseItem { Name = "Silver Fir Needle Essential Oil",   Unit = UnitMeasurement.UnitType.ml, TaxPercentage = 21, MinimumQuantityAlarm = 15, Categories = new List<Category> { categoryLookup["Essential Oils"] } },
                new WarehouseItem { Name = "Peppermint Essential Oil",          Unit = UnitMeasurement.UnitType.ml, TaxPercentage = 21, MinimumQuantityAlarm = 15, Categories = new List<Category> { categoryLookup["Essential Oils"] } },
                new WarehouseItem { Name = "Clary Sage Essential Oil",          Unit = UnitMeasurement.UnitType.ml, TaxPercentage = 21, MinimumQuantityAlarm = 15, Categories = new List<Category> { categoryLookup["Essential Oils"] } },
                new WarehouseItem { Name = "Mountain Pine Essential Oil",       Unit = UnitMeasurement.UnitType.ml, TaxPercentage = 21, MinimumQuantityAlarm = 15, Categories = new List<Category> { categoryLookup["Essential Oils"] } },
                new WarehouseItem { Name = "Lavender Essential Oil",            Unit = UnitMeasurement.UnitType.ml, TaxPercentage = 21, MinimumQuantityAlarm = 15, Categories = new List<Category> { categoryLookup["Essential Oils"] } },
                new WarehouseItem { Name = "Red Mandarin Essential Oil",        Unit = UnitMeasurement.UnitType.ml, TaxPercentage = 21, MinimumQuantityAlarm = 15, Categories = new List<Category> { categoryLookup["Essential Oils"] } },
                new WarehouseItem { Name = "Clove Essential Oil",               Unit = UnitMeasurement.UnitType.ml, TaxPercentage = 21, MinimumQuantityAlarm = 15, Categories = new List<Category> { categoryLookup["Essential Oils"] } },
                new WarehouseItem { Name = "Patchouli Essential Oil",           Unit = UnitMeasurement.UnitType.ml, TaxPercentage = 21, MinimumQuantityAlarm = 15, Categories = new List<Category> { categoryLookup["Essential Oils"] } },
                new WarehouseItem { Name = "Rosemary Essential Oil",            Unit = UnitMeasurement.UnitType.ml, TaxPercentage = 21, MinimumQuantityAlarm = 15, Categories = new List<Category> { categoryLookup["Essential Oils"] } },
                new WarehouseItem { Name = "Thyme Essential Oil",               Unit = UnitMeasurement.UnitType.ml, TaxPercentage = 21, MinimumQuantityAlarm = 15, Categories = new List<Category> { categoryLookup["Essential Oils"] } },
                new WarehouseItem { Name = "Frankincense Essential Oil",        Unit = UnitMeasurement.UnitType.ml, TaxPercentage = 21, MinimumQuantityAlarm = 15, Categories = new List<Category> { categoryLookup["Essential Oils"] } },
                new WarehouseItem { Name = "Cedarwood Essential Oil",           Unit = UnitMeasurement.UnitType.ml, TaxPercentage = 21, MinimumQuantityAlarm = 15, Categories = new List<Category> { categoryLookup["Essential Oils"] } },
                new WarehouseItem { Name = "Ylang Ylang Essential Oil",         Unit = UnitMeasurement.UnitType.ml, TaxPercentage = 21, MinimumQuantityAlarm = 15, Categories = new List<Category> { categoryLookup["Essential Oils"] } },
                new WarehouseItem { Name = "Black Pepper Essential Oil",        Unit = UnitMeasurement.UnitType.ml, TaxPercentage = 21, MinimumQuantityAlarm = 15, Categories = new List<Category> { categoryLookup["Essential Oils"] } },
                new WarehouseItem { Name = "Ginger Essential Oil",              Unit = UnitMeasurement.UnitType.ml, TaxPercentage = 21, MinimumQuantityAlarm = 15, Categories = new List<Category> { categoryLookup["Essential Oils"] } },
                new WarehouseItem { Name = "Lemongrass Essential Oil",          Unit = UnitMeasurement.UnitType.ml, TaxPercentage = 21, MinimumQuantityAlarm = 15, Categories = new List<Category> { categoryLookup["Essential Oils"] } },
                new WarehouseItem { Name = "Tea Tree Essential Oil",            Unit = UnitMeasurement.UnitType.ml, TaxPercentage = 21, MinimumQuantityAlarm = 15, Categories = new List<Category> { categoryLookup["Essential Oils"] } },
                new WarehouseItem { Name = "Cinnamon Leaf Essential Oil",       Unit = UnitMeasurement.UnitType.ml, TaxPercentage = 21, MinimumQuantityAlarm = 15, Categories = new List<Category> { categoryLookup["Essential Oils"] } },
                new WarehouseItem { Name = "Vetiver Essential Oil",             Unit = UnitMeasurement.UnitType.ml, TaxPercentage = 21, MinimumQuantityAlarm = 15, Categories = new List<Category> { categoryLookup["Essential Oils"] } },
                new WarehouseItem { Name = "Orange Essential Oil",              Unit = UnitMeasurement.UnitType.ml, TaxPercentage = 21, MinimumQuantityAlarm = 15, Categories = new List<Category> { categoryLookup["Essential Oils"] } },
                new WarehouseItem { Name = "Juniper Wood Essential Oil",        Unit = UnitMeasurement.UnitType.ml, TaxPercentage = 21, MinimumQuantityAlarm = 15, Categories = new List<Category> { categoryLookup["Essential Oils"] } },

                new WarehouseItem { Name = "Amyris Essential Oil (West Indian Sandalwood)", Unit = UnitMeasurement.UnitType.ml, TaxPercentage = 21, MinimumQuantityAlarm = 25, Categories = new List<Category> { categoryLookup["Essential Oils"] } },
                new WarehouseItem { Name = "Musk Sage - Clary Sage Essential Oil",          Unit = UnitMeasurement.UnitType.ml, TaxPercentage = 21, MinimumQuantityAlarm = 25, Categories = new List<Category> { categoryLookup["Essential Oils"] } },

                new WarehouseItem { Name = "Green Clay",    Unit = UnitMeasurement.UnitType.g, TaxPercentage = 21, MinimumQuantityAlarm = 100, Categories = new List<Category> { categoryLookup["Colorants & Pigments"] } },
                new WarehouseItem { Name = "Red Clay",      Unit = UnitMeasurement.UnitType.g, TaxPercentage = 21, MinimumQuantityAlarm = 100, Categories = new List<Category> { categoryLookup["Colorants & Pigments"] } },
                new WarehouseItem { Name = "Rose Clay",     Unit = UnitMeasurement.UnitType.g, TaxPercentage = 21, MinimumQuantityAlarm = 100, Categories = new List<Category> { categoryLookup["Colorants & Pigments"] } },
                new WarehouseItem { Name = "Yellow Clay",   Unit = UnitMeasurement.UnitType.g, TaxPercentage = 21, MinimumQuantityAlarm = 100, Categories = new List<Category> { categoryLookup["Colorants & Pigments"] } },
                new WarehouseItem { Name = "White Clay",    Unit = UnitMeasurement.UnitType.g, TaxPercentage = 21, MinimumQuantityAlarm = 100, Categories = new List<Category> { categoryLookup["Colorants & Pigments"] } },
                new WarehouseItem { Name = "Blue Clay",     Unit = UnitMeasurement.UnitType.g, TaxPercentage = 21, MinimumQuantityAlarm = 100, Categories = new List<Category> { categoryLookup["Colorants & Pigments"] } },
                new WarehouseItem { Name = "Purple Clay",   Unit = UnitMeasurement.UnitType.g, TaxPercentage = 21, MinimumQuantityAlarm = 100, Categories = new List<Category> { categoryLookup["Colorants & Pigments"] } },
                new WarehouseItem { Name = "Kaolin Clay",   Unit = UnitMeasurement.UnitType.g, TaxPercentage = 21, MinimumQuantityAlarm = 100, Categories = new List<Category> { categoryLookup["Colorants & Pigments"] } },

                new WarehouseItem { Name = "Black Carbon Plant Charcoal", Unit = UnitMeasurement.UnitType.g, TaxPercentage = 21, MinimumQuantityAlarm = 50, Categories = new List<Category> { categoryLookup["Colorants & Pigments"] } },

                new WarehouseItem { Name = "Paper Box (137x90x34mm)",   Unit = UnitMeasurement.UnitType.Pcs, TaxPercentage = 21, MinimumQuantityAlarm = 10, Categories = new List<Category> { categoryLookup["Packaging & Labels"] } },
                new WarehouseItem { Name = "Paper Box (162x154x52mm)",  Unit = UnitMeasurement.UnitType.Pcs, TaxPercentage = 21, MinimumQuantityAlarm = 10, Categories = new List<Category> { categoryLookup["Packaging & Labels"] } },
                new WarehouseItem { Name = "Labels (105x148mm)",        Unit = UnitMeasurement.UnitType.Pcs, TaxPercentage = 21, MinimumQuantityAlarm = 20, Categories = new List<Category> { categoryLookup["Packaging & Labels"] } },

                new WarehouseItem { Name = "Juniper Essential Oil",     Unit = UnitMeasurement.UnitType.ml, TaxPercentage = 21, MinimumQuantityAlarm = 50, Categories = new List<Category> { categoryLookup["Essential Oils"] } },
                new WarehouseItem { Name = "Pine Needle Essential Oil", Unit = UnitMeasurement.UnitType.ml, TaxPercentage = 21, MinimumQuantityAlarm = 50, Categories = new List<Category> { categoryLookup["Essential Oils"] } },
                new WarehouseItem { Name = "Tangerine Essential Oil",   Unit = UnitMeasurement.UnitType.ml, TaxPercentage = 21, MinimumQuantityAlarm = 50, Categories = new List<Category> { categoryLookup["Essential Oils"] } },
                
                new WarehouseItem { Name = "Aritha Soap Nut Powder", Unit = UnitMeasurement.UnitType.ml, TaxPercentage = 21, MinimumQuantityAlarm = 50, Categories = new List<Category> { categoryLookup["Herbs & Botanicals"] } },

            };
            _context.WarehouseItems.AddRange(warehouseItems);
            await _context.SaveChangesAsync();

            var itemLookup = _context.WarehouseItems
            .ToDictionary(item => item.Name, item => item.Id);

            // Vytvoření šarží (batches) pro každou skladovou položku
            var batches2022 = new List<Batch>
            {
                new Batch{ Name = "Labels (105x148mm)",         Supplier = "Top-obaly.cz",  PurchaseDate = new DateTime(2022, 5, 19),   ExpirationDate = null,  TaxPercentage = 21, AvailableQuantity = 400,    UnitPriceWithoutTax = 0.0239m,    WarehouseItemId = _context.WarehouseItems.FirstOrDefault(w => w.Name == "Labels (105x148mm)")?.Id ?? 0  },
                new Batch{ Name = "Paper Box (137x90x34mm)",    Supplier = "Top-obaly.cz",  PurchaseDate = new DateTime(2022, 5, 19),   ExpirationDate = null,  TaxPercentage = 21, AvailableQuantity = 40,     UnitPriceWithoutTax = 0.118m,    WarehouseItemId = _context.WarehouseItems.FirstOrDefault(w => w.Name == "Paper Box (137x90x34mm)")?.Id ?? 0  },
                new Batch{ Name = "Paper Box (162x154x52mm)",   Supplier = "Top-obaly.cz",  PurchaseDate = new DateTime(2022, 5, 19),   ExpirationDate = null,  TaxPercentage = 21, AvailableQuantity = 40,     UnitPriceWithoutTax = 0.203m,    WarehouseItemId = _context.WarehouseItems.FirstOrDefault(w => w.Name == "Paper Box (162x154x52mm)")?.Id ?? 0    },
            };

            var batchesMarch = new List<Batch>
            {
                new Batch { Name = "Clay, green, ultrafine (1102)",         Supplier = "Behawe", PurchaseDate = new DateTime(2025, 3, 6), ExpirationDate = new DateTime(2027, 3, 6), TaxPercentage = 21, AvailableQuantity = 250, UnitPriceWithoutTax = 0.022m,     WarehouseItemId = _context.WarehouseItems.FirstOrDefault(w => w.Name == "Green Clay")?.Id ?? 0 },
                new Batch { Name = "Clay, red, ultrafine (1105)",           Supplier = "Behawe", PurchaseDate = new DateTime(2025, 3, 6), ExpirationDate = new DateTime(2027, 3, 6), TaxPercentage = 21, AvailableQuantity = 250, UnitPriceWithoutTax = 0.022m,     WarehouseItemId = _context.WarehouseItems.FirstOrDefault(w => w.Name == "Red Clay")?.Id ?? 0 },
                new Batch { Name = "Clay, rose-colored, ultrafine (1108)",  Supplier = "Behawe", PurchaseDate = new DateTime(2025, 3, 6), ExpirationDate = new DateTime(2027, 3, 6), TaxPercentage = 21, AvailableQuantity = 250, UnitPriceWithoutTax = 0.0225m,    WarehouseItemId = _context.WarehouseItems.FirstOrDefault(w => w.Name == "Rose Clay")?.Id ?? 0 },
                new Batch { Name = "Clay, yellow, ultrafine (1111)",        Supplier = "Behawe", PurchaseDate = new DateTime(2025, 3, 6), ExpirationDate = new DateTime(2027, 3, 6), TaxPercentage = 21, AvailableQuantity = 250, UnitPriceWithoutTax = 0.022m,     WarehouseItemId = _context.WarehouseItems.FirstOrDefault(w => w.Name == "Yellow Clay")?.Id ?? 0 },
                new Batch { Name = "Clay, white, ultrafine (1128)",         Supplier = "Behawe", PurchaseDate = new DateTime(2025, 3, 6), ExpirationDate = new DateTime(2027, 3, 6), TaxPercentage = 21, AvailableQuantity = 250, UnitPriceWithoutTax = 0.0225m,    WarehouseItemId = _context.WarehouseItems.FirstOrDefault(w => w.Name == "White Clay")?.Id ?? 0 },
                new Batch { Name = "Clay, blue, extra fine (2640)",         Supplier = "Behawe", PurchaseDate = new DateTime(2025, 3, 6), ExpirationDate = new DateTime(2027, 3, 6), TaxPercentage = 21, AvailableQuantity = 250, UnitPriceWithoutTax = 0.0292m,    WarehouseItemId = _context.WarehouseItems.FirstOrDefault(w => w.Name == "Blue Clay")?.Id ?? 0 },
                new Batch { Name = "Lemongrass oil, pure (5047)",       Supplier = "Behawe", PurchaseDate = new DateTime(2025, 3, 6), ExpirationDate = new DateTime(2027, 3, 6), TaxPercentage = 21, AvailableQuantity = 50, UnitPriceWithoutTax = 0.0974m,         WarehouseItemId = _context.WarehouseItems.FirstOrDefault(w => w.Name == "Lemongrass Essential Oil")?.Id ?? 0 },
                new Batch { Name = "Tea tree oil, pure (5099)",         Supplier = "Behawe", PurchaseDate = new DateTime(2025, 3, 6), ExpirationDate = new DateTime(2027, 3, 6), TaxPercentage = 21, AvailableQuantity = 50, UnitPriceWithoutTax = 0.1184m,     WarehouseItemId = _context.WarehouseItems.FirstOrDefault(w => w.Name == "Tea Tree Essential Oil")?.Id ?? 0 },
                new Batch { Name = "Cinnamon leaf oil, pure (5117)",    Supplier = "Behawe", PurchaseDate = new DateTime(2025, 3, 6), ExpirationDate = new DateTime(2027, 3, 6), TaxPercentage = 21, AvailableQuantity = 50, UnitPriceWithoutTax = 0.1076m,     WarehouseItemId = _context.WarehouseItems.FirstOrDefault(w => w.Name == "Cinnamon Leaf Essential Oil")?.Id ?? 0 },
                new Batch { Name = "Vetiver oil, pure (5129)",          Supplier = "Behawe", PurchaseDate = new DateTime(2025, 3, 6), ExpirationDate = new DateTime(2027, 3, 6), TaxPercentage = 21, AvailableQuantity = 10, UnitPriceWithoutTax = 1.412m,      WarehouseItemId =  _context.WarehouseItems.FirstOrDefault(w => w.Name == "Vetiver Essential Oil")?.Id ?? 0 },
                new Batch { Name = "Juniper wood oil, natural (5237)",  Supplier = "Behawe", PurchaseDate = new DateTime(2025, 3, 6), ExpirationDate = new DateTime(2027, 3, 6), TaxPercentage = 21, AvailableQuantity = 50, UnitPriceWithoutTax = 0.1042m,     WarehouseItemId = _context.WarehouseItems.FirstOrDefault(w => w.Name == "Juniper Essential Oil")?.Id ?? 0 },
                new Batch { Name = "Ginger oil, pure (5247)",           Supplier = "Behawe", PurchaseDate = new DateTime(2025, 3, 6), ExpirationDate = new DateTime(2027, 3, 6), TaxPercentage = 21, AvailableQuantity = 10, UnitPriceWithoutTax = 0.429m,      WarehouseItemId =  _context.WarehouseItems.FirstOrDefault(w => w.Name == "Ginger Essential Oil")?.Id ?? 0 },
                new Batch { Name = "Vegetable Charcoal (Black Carbon) (1755)",          Supplier = "Behawe", PurchaseDate = new DateTime(2025, 3, 6), ExpirationDate = new DateTime(2027, 3, 6), TaxPercentage = 21, AvailableQuantity = 50, UnitPriceWithoutTax = 0.1328m,     WarehouseItemId = _context.WarehouseItems.FirstOrDefault(w => w.Name == "Black Carbon Plant Charcoal")?.Id ?? 0 },
                new Batch { Name = "Amyris oil (West Indian sandalwood) pure (5095)",   Supplier = "Behawe", PurchaseDate = new DateTime(2025, 3, 6), ExpirationDate = new DateTime(2027, 3, 6), TaxPercentage = 21, AvailableQuantity = 30, UnitPriceWithoutTax = 0.2367m,     WarehouseItemId = _context.WarehouseItems.FirstOrDefault(w => w.Name == "Amyris Essential Oil (West Indian Sandalwood)")?.Id ?? 0 },
                new Batch { Name = "Orange oil, sweet, extra strong, natural (5205)",   Supplier = "Behawe", PurchaseDate = new DateTime(2025, 3, 6), ExpirationDate = new DateTime(2027, 3, 6), TaxPercentage = 21, AvailableQuantity = 50, UnitPriceWithoutTax = 0.0974m,     WarehouseItemId = _context.WarehouseItems.FirstOrDefault(w => w.Name == "Orange Essential Oil")?.Id ?? 0 },
                new Batch { Name = "Sweet Almond Oil, cold-pressed, organic (1895)",    Supplier = "Behawe", PurchaseDate = new DateTime(2025, 3, 6), ExpirationDate = new DateTime(2027, 3, 6), TaxPercentage = 21, AvailableQuantity = 100, UnitPriceWithoutTax = 0.0486m,    WarehouseItemId = _context.WarehouseItems.FirstOrDefault(w => w.Name == "Sweet Almond Oil")?.Id ?? 0 },
                new Batch { Name = "Jojoba Oil, cold-pressed, organic (1985)",          Supplier = "Behawe", PurchaseDate = new DateTime(2025, 3, 6), ExpirationDate = new DateTime(2027, 3, 6), TaxPercentage = 21, AvailableQuantity = 100, UnitPriceWithoutTax = 0.0597m,    WarehouseItemId = _context.WarehouseItems.FirstOrDefault(w => w.Name == "Jojoba Oil")?.Id ?? 0 },
                new Batch { Name = "Argan oil, cold-pressed, organic (2083)",           Supplier = "Behawe", PurchaseDate = new DateTime(2025, 3, 6), ExpirationDate = new DateTime(2027, 3, 6), TaxPercentage = 21, AvailableQuantity = 100, UnitPriceWithoutTax = 0.1112m,    WarehouseItemId = _context.WarehouseItems.FirstOrDefault(w => w.Name == "Argan Oil")?.Id ?? 0 }
            };
            
            var batchesFebruary = new List<Batch>
            {
                new Batch { Name = "Coconut oil, refined (1945)",       Supplier = "Behawe", PurchaseDate = new DateTime(2025, 2, 3), ExpirationDate = new DateTime(2027, 2, 3), TaxPercentage = 21, AvailableQuantity = 2, UnitPriceWithoutTax = 10.185m,      WarehouseItemId = _context.WarehouseItems.FirstOrDefault(w => w.Name == "Coconut Oil Butter")?.Id ?? 0 },
                new Batch { Name = "Olive oil, extra virgin (1995)",    Supplier = "Behawe", PurchaseDate = new DateTime(2025, 2, 3), ExpirationDate = new DateTime(2027, 2, 3), TaxPercentage = 21, AvailableQuantity = 2, UnitPriceWithoutTax = 17.615m,      WarehouseItemId = _context.WarehouseItems.FirstOrDefault(w => w.Name == "Olive Oil Extra Virgin")?.Id ?? 0 },
                new Batch { Name = "Shea butter refined (2010)",        Supplier = "Behawe", PurchaseDate = new DateTime(2025, 2, 3), ExpirationDate = new DateTime(2027, 2, 3), TaxPercentage = 21, AvailableQuantity = 1, UnitPriceWithoutTax = 16.26m,       WarehouseItemId = _context.WarehouseItems.FirstOrDefault(w => w.Name == "Shea Butter Organic")?.Id ?? 0 },
                new Batch { Name = "Bergamot Essential Oil (5007)",     Supplier = "Behawe", PurchaseDate = new DateTime(2025, 2, 3), ExpirationDate = new DateTime(2027, 2, 3), TaxPercentage = 21, AvailableQuantity = 50, UnitPriceWithoutTax = 0.0992m,     WarehouseItemId = _context.WarehouseItems.FirstOrDefault(w => w.Name == "Bergamot Essential Oil")?.Id ?? 0 },
                new Batch { Name = "Pine Needle Essential Oil (5016)",  Supplier = "Behawe", PurchaseDate = new DateTime(2025, 2, 3), ExpirationDate = new DateTime(2027, 2, 3), TaxPercentage = 21, AvailableQuantity = 10, UnitPriceWithoutTax = 0.202m,      WarehouseItemId = _context.WarehouseItems.FirstOrDefault(w => w.Name == "Pine Needle Essential Oil")?.Id ?? 0 },
                new Batch { Name = "Peppermint Essential Oil (5027)",   Supplier = "Behawe", PurchaseDate = new DateTime(2025, 2, 3), ExpirationDate = new DateTime(2027, 2, 3), TaxPercentage = 21, AvailableQuantity = 50, UnitPriceWithoutTax = 0.1126m,     WarehouseItemId = _context.WarehouseItems.FirstOrDefault(w => w.Name == "Peppermint Essential Oil")?.Id ?? 0 },
                new Batch { Name = "Clary Sage Essential Oil (5031)",   Supplier = "Behawe", PurchaseDate = new DateTime(2025, 2, 3), ExpirationDate = new DateTime(2027, 2, 3), TaxPercentage = 21, AvailableQuantity = 30, UnitPriceWithoutTax = 0.4117m,     WarehouseItemId = _context.WarehouseItems.FirstOrDefault(w => w.Name == "Clary Sage Essential Oil")?.Id ?? 0 },
                new Batch { Name = "Lavender Essential Oil (5045)",     Supplier = "Behawe", PurchaseDate = new DateTime(2025, 2, 3), ExpirationDate = new DateTime(2027, 2, 3), TaxPercentage = 21, AvailableQuantity = 50, UnitPriceWithoutTax = 0.1278m,     WarehouseItemId = _context.WarehouseItems.FirstOrDefault(w => w.Name == "Lavender Essential Oil")?.Id ?? 0 },
                new Batch { Name = "Mandarin oil red, pure (5053)",     Supplier = "Behawe", PurchaseDate = new DateTime(2025, 2, 3), ExpirationDate = new DateTime(2027, 2, 3), TaxPercentage = 21, AvailableQuantity = 30, UnitPriceWithoutTax = 0.175m,      WarehouseItemId = _context.WarehouseItems.FirstOrDefault(w => w.Name == "Red Mandarin Essential Oil")?.Id ?? 0 },
                new Batch { Name = "Clove oil extra, pure (5061)",      Supplier = "Behawe", PurchaseDate = new DateTime(2025, 2, 3), ExpirationDate = new DateTime(2027, 2, 3), TaxPercentage = 21, AvailableQuantity = 50, UnitPriceWithoutTax = 0.0992m,     WarehouseItemId = _context.WarehouseItems.FirstOrDefault(w => w.Name == "Clove Essential Oil")?.Id ?? 0 },
                new Batch { Name = "Patchouli oil, pure (5071)",        Supplier = "Behawe", PurchaseDate = new DateTime(2025, 2, 3), ExpirationDate = new DateTime(2027, 2, 3), TaxPercentage = 21, AvailableQuantity = 30, UnitPriceWithoutTax = 0.4007m,     WarehouseItemId = _context.WarehouseItems.FirstOrDefault(w => w.Name == "Patchouli Essential Oil")?.Id ?? 0 },
                new Batch { Name = "Rosemary oil, natural (5089)",      Supplier = "Behawe", PurchaseDate = new DateTime(2025, 2, 3), ExpirationDate = new DateTime(2027, 2, 3), TaxPercentage = 21, AvailableQuantity = 50, UnitPriceWithoutTax = 0.0966m,     WarehouseItemId = _context.WarehouseItems.FirstOrDefault(w => w.Name == "Rosemary Essential Oil")?.Id ?? 0 },
                new Batch { Name = "Thyme oil, natural (5105)",         Supplier = "Behawe", PurchaseDate = new DateTime(2025, 2, 3), ExpirationDate = new DateTime(2027, 2, 3), TaxPercentage = 21, AvailableQuantity = 30, UnitPriceWithoutTax = 0.136m,      WarehouseItemId = _context.WarehouseItems.FirstOrDefault(w => w.Name == "Thyme Essential Oil")?.Id ?? 0 },
                new Batch { Name = "Black pepper oil, pure (5219)",     Supplier = "Behawe", PurchaseDate = new DateTime(2025, 2, 3), ExpirationDate = new DateTime(2027, 2, 3), TaxPercentage = 21, AvailableQuantity = 30, UnitPriceWithoutTax = 0.2143m,     WarehouseItemId = _context.WarehouseItems.FirstOrDefault(w => w.Name == "Black Pepper Essential Oil")?.Id ?? 0 },
                new Batch { Name = "Ginger oil, pure (5247)",           Supplier = "Behawe", PurchaseDate = new DateTime(2025, 2, 3), ExpirationDate = new DateTime(2027, 2, 3), TaxPercentage = 21, AvailableQuantity = 10, UnitPriceWithoutTax = 0.429m,      WarehouseItemId = _context.WarehouseItems.FirstOrDefault(w => w.Name == "Ginger Essential Oil")?.Id ?? 0 },
                new Batch { Name = "Jasmine Essential Oil Blend (5001)",        Supplier = "Behawe", PurchaseDate = new DateTime(2025, 2, 3), ExpirationDate = new DateTime(2027, 2, 3), TaxPercentage = 21, AvailableQuantity = 10, UnitPriceWithoutTax = 0.584m, WarehouseItemId = _context.WarehouseItems.FirstOrDefault(w => w.Name == "Jasmine Essential Oil Blend")?.Id ?? 0 },
                new Batch { Name = "Frankincense oil, pure (5109)",             Supplier = "Behawe", PurchaseDate = new DateTime(2025, 2, 3), ExpirationDate = new DateTime(2027, 2, 3), TaxPercentage = 21, AvailableQuantity = 30, UnitPriceWithoutTax = 0.325m, WarehouseItemId = _context.WarehouseItems.FirstOrDefault(w => w.Name == "Frankincense Essential Oil")?.Id ?? 0 },
                new Batch { Name = "Cedarwood oil (Texas cedar) pure (5113)",   Supplier = "Behawe", PurchaseDate = new DateTime(2025, 2, 3), ExpirationDate = new DateTime(2027, 2, 3), TaxPercentage = 21, AvailableQuantity = 50, UnitPriceWithoutTax = 0.089m, WarehouseItemId = _context.WarehouseItems.FirstOrDefault(w => w.Name == "Cedarwood Essential Oil")?.Id ?? 0 },
                new Batch { Name = "Ylang-Ylang II oil, natural (5115)",        Supplier = "Behawe", PurchaseDate = new DateTime(2025, 2, 3), ExpirationDate = new DateTime(2027, 2, 3), TaxPercentage = 21, AvailableQuantity = 30, UnitPriceWithoutTax = 0.3753m, WarehouseItemId = _context.WarehouseItems.FirstOrDefault(w => w.Name == "Ylang Ylang Essential Oil")?.Id ?? 0 },
                new Batch { Name = "Mountain Pine Essential Oil (5041)",        Supplier = "Behawe", PurchaseDate = new DateTime(2025, 2, 3), ExpirationDate = new DateTime(2027, 2, 3), TaxPercentage = 21, AvailableQuantity = 50, UnitPriceWithoutTax = 0.11m,   WarehouseItemId = _context.WarehouseItems.FirstOrDefault(w => w.Name == "Mountain Pine Essential Oil")?.Id ?? 0 },
                new Batch { Name = "Castor oil, cold pressed, native, PH. Eur. 6.0 (1925)",     Supplier = "Behawe", PurchaseDate = new DateTime(2025, 2, 3), ExpirationDate = new DateTime(2027, 2, 3), TaxPercentage = 21, AvailableQuantity = 500, UnitPriceWithoutTax = 0.01236m, WarehouseItemId = _context.WarehouseItems.FirstOrDefault(w => w.Name == "Castor Oil Cold Pressed")?.Id ?? 0 }

            };

            var batchesJanuary = new List<Batch>
            {
                new Batch { Name = "Distilled Water (YouWish)",         Supplier = "YouWish", PurchaseDate = new DateTime(2025, 1, 23), ExpirationDate = new DateTime(2027, 1, 23), TaxPercentage = 21, AvailableQuantity = 2, UnitPriceWithoutTax = 2.25m, WarehouseItemId = _context.WarehouseItems.FirstOrDefault(w => w.Name == "Distilled Water")?.Id ?? 0 },
                new Batch { Name = "Ginger Essential Oil (YouWish)",    Supplier = "YouWish", PurchaseDate = new DateTime(2025, 1, 23), ExpirationDate = new DateTime(2027, 1, 23), TaxPercentage = 21, AvailableQuantity = 10, UnitPriceWithoutTax = 0.595m, WarehouseItemId = _context.WarehouseItems.FirstOrDefault(w => w.Name == "Ginger Essential Oil")?.Id ?? 0 },
                new Batch { Name = "Thyme Essential Oil (YouWish)",     Supplier = "YouWish", PurchaseDate = new DateTime(2025, 1, 23), ExpirationDate = new DateTime(2027, 1, 23), TaxPercentage = 21, AvailableQuantity = 10, UnitPriceWithoutTax = 0.595m, WarehouseItemId = _context.WarehouseItems.FirstOrDefault(w => w.Name == "Thyme Essential Oil")?.Id ?? 0 },
                new Batch { Name = "Cedarwood Essential Oil (YouWish)", Supplier = "YouWish", PurchaseDate = new DateTime(2025, 1, 23), ExpirationDate = new DateTime(2027, 1, 23), TaxPercentage = 21, AvailableQuantity = 10, UnitPriceWithoutTax = 0.595m, WarehouseItemId = _context.WarehouseItems.FirstOrDefault(w => w.Name == "Cedarwood Essential Oil")?.Id ?? 0 },
                new Batch { Name = "Bergamot Essential Oil (YouWish)",  Supplier = "YouWish", PurchaseDate = new DateTime(2025, 1, 23), ExpirationDate = new DateTime(2027, 1, 23), TaxPercentage = 21, AvailableQuantity = 10, UnitPriceWithoutTax = 0.395m, WarehouseItemId = _context.WarehouseItems.FirstOrDefault(w => w.Name == "Bergamot Essential Oil")?.Id ?? 0 },
                new Batch { Name = "Rosemary Essential Oil (YouWish)",  Supplier = "YouWish", PurchaseDate = new DateTime(2025, 1, 23), ExpirationDate = new DateTime(2027, 1, 23), TaxPercentage = 21, AvailableQuantity = 10, UnitPriceWithoutTax = 0.395m, WarehouseItemId = _context.WarehouseItems.FirstOrDefault(w => w.Name == "Rosemary Essential Oil")?.Id ?? 0 },
                new Batch { Name = "Lavender Essential Oil (YouWish)",  Supplier = "YouWish", PurchaseDate = new DateTime(2025, 1, 23), ExpirationDate = new DateTime(2027, 1, 23), TaxPercentage = 21, AvailableQuantity = 30, UnitPriceWithoutTax = 0.265m, WarehouseItemId = _context.WarehouseItems.FirstOrDefault(w => w.Name == "Lavender Essential Oil")?.Id ?? 0 },
                new Batch { Name = "Patchouli Essential Oil (YouWish)", Supplier = "YouWish", PurchaseDate = new DateTime(2025, 1, 23), ExpirationDate = new DateTime(2027, 1, 23), TaxPercentage = 21, AvailableQuantity = 30, UnitPriceWithoutTax = 0.565m, WarehouseItemId = _context.WarehouseItems.FirstOrDefault(w => w.Name == "Patchouli Essential Oil")?.Id ?? 0 },
                new Batch { Name = "French Green Clay (YouWish)",       Supplier = "YouWish", PurchaseDate = new DateTime(2025, 1, 23), ExpirationDate = new DateTime(2027, 1, 23), TaxPercentage = 21, AvailableQuantity = 10, UnitPriceWithoutTax = 0.195m, WarehouseItemId = _context.WarehouseItems.FirstOrDefault(w => w.Name == "Green Clay")?.Id ?? 0 },
                new Batch { Name = "Kaolin Clay (YouWish)",             Supplier = "YouWish", PurchaseDate = new DateTime(2025, 1, 23), ExpirationDate = new DateTime(2027, 1, 23), TaxPercentage = 21, AvailableQuantity = 10, UnitPriceWithoutTax = 0.095m, WarehouseItemId = _context.WarehouseItems.FirstOrDefault(w => w.Name == "Kaolin Clay")?.Id ?? 0 },
                new Batch { Name = "Aritha Soap Nut Powder (YouWish)",  Supplier = "YouWish", PurchaseDate = new DateTime(2025, 1, 23), ExpirationDate = new DateTime(2027, 1, 23), TaxPercentage = 21, AvailableQuantity = 10, UnitPriceWithoutTax = 0.245m, WarehouseItemId = _context.WarehouseItems.FirstOrDefault(w => w.Name == "Aritha Soap Nut Powder")?.Id ?? 0 },
                new Batch { Name = "Red Brazilian Clay (YouWish)",      Supplier = "YouWish", PurchaseDate = new DateTime(2025, 1, 23), ExpirationDate = new DateTime(2027, 1, 23), TaxPercentage = 21, AvailableQuantity = 10, UnitPriceWithoutTax = 0.225m, WarehouseItemId = _context.WarehouseItems.FirstOrDefault(w => w.Name == "Red Clay")?.Id ?? 0 },

                new Batch { Name = "Cambrian Blue Clay Powder (YouWish)",   Supplier = "YouWish", PurchaseDate = new DateTime(2025, 1, 23), ExpirationDate = new DateTime(2027, 1, 23), TaxPercentage = 21, AvailableQuantity = 10, UnitPriceWithoutTax = 0.225m, WarehouseItemId = _context.WarehouseItems.FirstOrDefault(w => w.Name == "Blue Clay")?.Id ?? 0 },
                new Batch { Name = "Brazilian Clay - Yellow (YouWish)",     Supplier = "YouWish", PurchaseDate = new DateTime(2025, 1, 23), ExpirationDate = new DateTime(2027, 1, 23), TaxPercentage = 21, AvailableQuantity = 10, UnitPriceWithoutTax = 0.225m, WarehouseItemId = _context.WarehouseItems.FirstOrDefault(w => w.Name == "Yellow Clay")?.Id ?? 0 },
                new Batch { Name = "Brazilian Clay - Purple (YouWish)",     Supplier = "YouWish", PurchaseDate = new DateTime(2025, 1, 23), ExpirationDate = new DateTime(2027, 1, 23), TaxPercentage = 21, AvailableQuantity = 10, UnitPriceWithoutTax = 0.225m, WarehouseItemId = _context.WarehouseItems.FirstOrDefault(w => w.Name == "Purple Clay")?.Id ?? 0 },

                new Batch { Name = "Sodium Hydroxide Micro Pearls (YouWish)",   Supplier = "YouWish", PurchaseDate = new DateTime(2025, 1, 23), ExpirationDate = new DateTime(2027, 1, 23), TaxPercentage = 21, AvailableQuantity = 1, UnitPriceWithoutTax = 7.95m, WarehouseItemId = _context.WarehouseItems.FirstOrDefault(w => w.Name == "Sodium Hydroxide")?.Id ?? 0 },
                new Batch { Name = "Coconut Oil Butter - Organic (YouWish)",    Supplier = "YouWish", PurchaseDate = new DateTime(2025, 1, 23), ExpirationDate = new DateTime(2027, 1, 23), TaxPercentage = 21, AvailableQuantity = 2, UnitPriceWithoutTax = 13.5m, WarehouseItemId = _context.WarehouseItems.FirstOrDefault(w => w.Name == "Coconut Oil Butter")?.Id ?? 0 },
                new Batch { Name = "Shea Butter Organic - Refined (YouWish)",   Supplier = "YouWish", PurchaseDate = new DateTime(2025, 1, 23), ExpirationDate = new DateTime(2027, 1, 23), TaxPercentage = 21, AvailableQuantity = 1, UnitPriceWithoutTax = 14.95m, WarehouseItemId = _context.WarehouseItems.FirstOrDefault(w => w.Name == "Shea Butter Organic")?.Id ?? 0 },
                new Batch { Name = "Olive Oil Extra Virgin (YouWish)",          Supplier = "YouWish", PurchaseDate = new DateTime(2025, 1, 23), ExpirationDate = new DateTime(2027, 1, 23), TaxPercentage = 21, AvailableQuantity = 4, UnitPriceWithoutTax = 16.95m, WarehouseItemId = _context.WarehouseItems.FirstOrDefault(w => w.Name == "Olive Oil Extra Virgin")?.Id ?? 0 },
                new Batch { Name = "Castor Oil Cold Pressed (YouWish)",         Supplier = "YouWish", PurchaseDate = new DateTime(2025, 1, 23), ExpirationDate = new DateTime(2027, 1, 23), TaxPercentage = 21, AvailableQuantity = 500, UnitPriceWithoutTax = 0.0143m, WarehouseItemId = _context.WarehouseItems.FirstOrDefault(w => w.Name == "Castor Oil Cold Pressed")?.Id ?? 0 },
                new Batch { Name = "Tangerine Essential Oil (YouWish)",         Supplier = "YouWish", PurchaseDate = new DateTime(2025, 1, 23), ExpirationDate = new DateTime(2027, 1, 23), TaxPercentage = 21, AvailableQuantity = 10, UnitPriceWithoutTax = 0.495m, WarehouseItemId = _context.WarehouseItems.FirstOrDefault(w => w.Name == "Tangerine Essential Oil")?.Id ?? 0 },
                new Batch { Name = "Pine Needle Essential Oil (YouWish)",       Supplier = "YouWish", PurchaseDate = new DateTime(2025, 1, 23), ExpirationDate = new DateTime(2027, 1, 23), TaxPercentage = 21, AvailableQuantity = 10, UnitPriceWithoutTax = 0.395m, WarehouseItemId = _context.WarehouseItems.FirstOrDefault(w => w.Name == "Pine Needle Essential Oil")?.Id ?? 0 },
                new Batch { Name = "Ylang Ylang Essential Oil (YouWish)",       Supplier = "YouWish", PurchaseDate = new DateTime(2025, 1, 23), ExpirationDate = new DateTime(2027, 1, 23), TaxPercentage = 21, AvailableQuantity = 10, UnitPriceWithoutTax = 0.795m, WarehouseItemId = _context.WarehouseItems.FirstOrDefault(w => w.Name == "Ylang Ylang Essential Oil")?.Id ?? 0 },
                new Batch { Name = "Black Pepper Essential Oil (YouWish)",      Supplier = "YouWish", PurchaseDate = new DateTime(2025, 1, 23), ExpirationDate = new DateTime(2027, 1, 23), TaxPercentage = 21, AvailableQuantity = 30, UnitPriceWithoutTax = 0.598m, WarehouseItemId = _context.WarehouseItems.FirstOrDefault(w => w.Name == "Black Pepper Essential Oil")?.Id ?? 0 },
                new Batch { Name = "Peppermint Essential Oil (YouWish)",        Supplier = "YouWish", PurchaseDate = new DateTime(2025, 1, 23), ExpirationDate = new DateTime(2027, 1, 23), TaxPercentage = 21, AvailableQuantity = 10, UnitPriceWithoutTax = 0.395m, WarehouseItemId = _context.WarehouseItems.FirstOrDefault(w => w.Name == "Peppermint Essential Oil")?.Id ?? 0 },

                new Batch { Name = "Musk Sage - Clary Sage Essential Oil (YouWish)", Supplier = "YouWish", PurchaseDate = new DateTime(2025, 1, 23), ExpirationDate = new DateTime(2027, 1, 23), TaxPercentage = 21, AvailableQuantity = 10, UnitPriceWithoutTax = 0.495m, WarehouseItemId = _context.WarehouseItems.FirstOrDefault(w => w.Name == "Musk Sage - Clary Sage Essential Oil")?.Id ?? 0 }
            };


            _context.Batches.AddRange(batches2022);
            _context.Batches.AddRange(batchesMarch);
            _context.Batches.AddRange(batchesJanuary);
            _context.Batches.AddRange(batchesFebruary);
            await _context.SaveChangesAsync();

            var warehouseItemsForRecipe = await _context.WarehouseItems.ToListAsync();

            // Vytvoření receptů
            var recipes = new List<Recipe>
            {
                new Recipe
                {
                    Name = "MIDSUMMER",
                    ImageUrl = "-",
                    BatchSize = 11,
                    DaysOfCure = 40,
                    ProductType = ProductType.Soap,
                    Note = "Classic MIDSUMMER aroma",
                    Ingredients = new List<RecipeIngredient>
                    {
                        new RecipeIngredient { Quantity = 0.11m,    Unit = warehouseItemsForRecipe.FirstOrDefault(x => x.Name == "Sodium Hydroxide") ?.Unit ?? UnitMeasurement.UnitType.L,      WarehouseItemId = warehouseItemsForRecipe.FirstOrDefault(x => x.Name == "Sodium Hydroxide") ?.Id ?? 0},
                        new RecipeIngredient { Quantity = 0.2m,     Unit = warehouseItemsForRecipe.FirstOrDefault(x => x.Name == "Distilled water") ?.Unit ?? UnitMeasurement.UnitType.L,       WarehouseItemId = warehouseItemsForRecipe.FirstOrDefault(x => x.Name == "Distilled water") ?.Id ?? 0},
                        new RecipeIngredient { Quantity = 0.400m,   Unit = warehouseItemsForRecipe.FirstOrDefault(x => x.Name == "Olive Oil Extra Virgin") ?.Unit ?? UnitMeasurement.UnitType.L,WarehouseItemId = warehouseItemsForRecipe.FirstOrDefault(x => x.Name == "Olive Oil Extra Virgin") ?.Id ?? 0},
                        new RecipeIngredient { Quantity = 0.2m,     Unit = warehouseItemsForRecipe.FirstOrDefault(x => x.Name == "Coconut Oil Butter") ?.Unit ?? UnitMeasurement.UnitType.L,    WarehouseItemId = warehouseItemsForRecipe.FirstOrDefault(x => x.Name == "Coconut Oil Butter") ?.Id ?? 0},
                        new RecipeIngredient { Quantity = 0.15m,    Unit = warehouseItemsForRecipe.FirstOrDefault(x => x.Name == "Shea Butter Organic") ?.Unit ?? UnitMeasurement.UnitType.L,   WarehouseItemId = warehouseItemsForRecipe.FirstOrDefault(x => x.Name == "Shea Butter Organic") ?.Id ?? 0},
                        new RecipeIngredient { Quantity = 50,       Unit = warehouseItemsForRecipe.FirstOrDefault(x => x.Name == "Castor Oil Cold Pressed") ?.Unit ?? UnitMeasurement.UnitType.L,  WarehouseItemId = warehouseItemsForRecipe.FirstOrDefault(x => x.Name == "Castor Oil Cold Pressed") ?.Id ?? 0},
                        
                        new RecipeIngredient { Quantity = 10,       Unit = warehouseItemsForRecipe.FirstOrDefault(x => x.Name == "Red Clay") ?.Unit ?? UnitMeasurement.UnitType.L,                      WarehouseItemId = warehouseItemsForRecipe.FirstOrDefault(x => x.Name == "Red Clay") ?.Id ?? 0},
                        new RecipeIngredient { Quantity = 14,       Unit = warehouseItemsForRecipe.FirstOrDefault(x => x.Name == "Tangerine Essential Oil") ?.Unit ?? UnitMeasurement.UnitType.L,       WarehouseItemId = warehouseItemsForRecipe.FirstOrDefault(x => x.Name == "Tangerine Essential Oil") ?.Id ?? 0},
                        new RecipeIngredient { Quantity = 2,        Unit = warehouseItemsForRecipe.FirstOrDefault(x => x.Name == "Clove Essential Oil") ?.Unit ?? UnitMeasurement.UnitType.L,           WarehouseItemId = warehouseItemsForRecipe.FirstOrDefault(x => x.Name == "Clove Essential Oil") ?.Id ?? 0},
                        new RecipeIngredient { Quantity = 8,        Unit = warehouseItemsForRecipe.FirstOrDefault(x => x.Name == "Frankincense Essential Oil") ?.Unit ?? UnitMeasurement.UnitType.L,    WarehouseItemId = warehouseItemsForRecipe.FirstOrDefault(x => x.Name == "Frankincense Essential Oil") ?.Id ?? 0},
                }
            },
            new Recipe
            {
                Name = "WILDFLOWER",
                ImageUrl = "-",
                BatchSize = 11,
                DaysOfCure = 30,
                ProductType = ProductType.Soap,
                Note = "Classic WILDFLOWER aroma",
                Ingredients = new List<RecipeIngredient>
                                    {
                    new RecipeIngredient { Quantity = 0.11m,    Unit = warehouseItemsForRecipe.FirstOrDefault(x => x.Name == "Sodium Hydroxide") ?.Unit ?? UnitMeasurement.UnitType.L,      WarehouseItemId = warehouseItemsForRecipe.FirstOrDefault(x => x.Name == "Sodium Hydroxide") ?.Id ?? 0},
                    new RecipeIngredient { Quantity = 0.2m,     Unit = warehouseItemsForRecipe.FirstOrDefault(x => x.Name == "Distilled water") ?.Unit ?? UnitMeasurement.UnitType.L,       WarehouseItemId = warehouseItemsForRecipe.FirstOrDefault(x => x.Name == "Distilled water") ?.Id ?? 0},
                    new RecipeIngredient { Quantity = 0.400m,   Unit = warehouseItemsForRecipe.FirstOrDefault(x => x.Name == "Olive Oil Extra Virgin") ?.Unit ?? UnitMeasurement.UnitType.L,WarehouseItemId = warehouseItemsForRecipe.FirstOrDefault(x => x.Name == "Olive Oil Extra Virgin") ?.Id ?? 0},
                    new RecipeIngredient { Quantity = 0.2m,     Unit = warehouseItemsForRecipe.FirstOrDefault(x => x.Name == "Coconut Oil Butter") ?.Unit ?? UnitMeasurement.UnitType.L,    WarehouseItemId = warehouseItemsForRecipe.FirstOrDefault(x => x.Name == "Coconut Oil Butter") ?.Id ?? 0},
                    new RecipeIngredient { Quantity = 0.15m,    Unit = warehouseItemsForRecipe.FirstOrDefault(x => x.Name == "Shea Butter Organic") ?.Unit ?? UnitMeasurement.UnitType.L,   WarehouseItemId = warehouseItemsForRecipe.FirstOrDefault(x => x.Name == "Shea Butter Organic") ?.Id ?? 0},
                    new RecipeIngredient { Quantity = 50,       Unit = warehouseItemsForRecipe.FirstOrDefault(x => x.Name == "Castor Oil Cold Pressed") ?.Unit ?? UnitMeasurement.UnitType.L,  WarehouseItemId = warehouseItemsForRecipe.FirstOrDefault(x => x.Name == "Castor Oil Cold Pressed") ?.Id ?? 0},

                    new RecipeIngredient { Quantity = 10,       Unit = warehouseItemsForRecipe.FirstOrDefault(x => x.Name == "Purple Clay") ?.Unit ?? UnitMeasurement.UnitType.L,               WarehouseItemId = warehouseItemsForRecipe.FirstOrDefault(x => x.Name == "Purple Clay") ?.Id ?? 0},
                    new RecipeIngredient { Quantity = 10,       Unit = warehouseItemsForRecipe.FirstOrDefault(x => x.Name == "Lavender Essential Oil") ?.Unit ?? UnitMeasurement.UnitType.L,    WarehouseItemId = warehouseItemsForRecipe.FirstOrDefault(x => x.Name == "Lavender Essential Oil") ?.Id ?? 0},
                    new RecipeIngredient { Quantity = 7,        Unit = warehouseItemsForRecipe.FirstOrDefault(x => x.Name == "Patchouli Essential Oil") ?.Unit ?? UnitMeasurement.UnitType.L,   WarehouseItemId = warehouseItemsForRecipe.FirstOrDefault(x => x.Name == "Patchouli Essential Oil") ?.Id ?? 0},
                    new RecipeIngredient { Quantity = 7,        Unit = warehouseItemsForRecipe.FirstOrDefault(x => x.Name == "Pine Needle Essential Oil") ?.Unit ?? UnitMeasurement.UnitType.L,  WarehouseItemId = warehouseItemsForRecipe.FirstOrDefault(x => x.Name == "Pine Needle Essential Oil") ?.Id ?? 0},
                }
            }};
            _context.Recipes.AddRange(recipes);
            await _context.SaveChangesAsync();
            
            return RedirectToAction(nameof(Index));
        }

        private void DeleteImagesInUploadFolder()
        {
            // Cesta ke složce "uploads"
            var uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
            var thumbnailsPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads/thumbnails");
            DeleteFilesInFolder(uploadsPath);
            DeleteFilesInFolder(thumbnailsPath);
        }

        private void DeleteFilesInFolder(string folder)
        {
            if (Directory.Exists(folder))
            {
                foreach (var file in Directory.GetFiles(folder))
                {
                    try
                    {
                        System.IO.File.Delete(file);
                    }
                    catch { }
                }
            }
        }
    }
}
