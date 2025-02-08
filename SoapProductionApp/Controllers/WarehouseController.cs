using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SoapProductionApp.Data;
using SoapProductionApp.Models;
using SoapProductionApp.Models.Warehouse;
using System.Linq;
using System.Threading.Tasks;

namespace SoapProductionApp.Controllers
{
    [Authorize]
    public class WarehouseController : Controller
    {
        private readonly ApplicationDbContext _context;

        public WarehouseController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var items = await _context.WarehouseItems
                .Include(i => i.Batches)
                .Include(i => i.Categories)
                .Include(i => i.Unit).ToListAsync();
            return View(items);
        }

        public async Task<IActionResult> Create()
        {
            var viewModel = new WarehouseItemCreateViewModel
            {
                AvailableUnits = _context.Units.ToList(),
                AvailableCategories = _context.Categories.ToList()
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(WarehouseItemCreateViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var warehouseItem = new WarehouseItem
                {
                    Name = viewModel.Name,
                    Quantity = viewModel.Quantity,
                    UnitId = viewModel.SelectedUnitId,
                    PricePerUnit = viewModel.PricePerUnit,
                    TaxPercentage = viewModel.TaxPercentage,
                    MinQuantity = viewModel.MinQuantity,
                    Supplier = viewModel.Supplier,
                    Notes = viewModel.Notes,
                    Categories = _context.Categories.Where(c => viewModel.SelectedCategoryIds.Contains(c.Id)).ToList()
                };

                _context.WarehouseItems.Add(warehouseItem);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            // Pokud jsou chyby, znovu naplníme seznamy
            viewModel.AvailableUnits = _context.Units.ToList();
            viewModel.AvailableCategories = _context.Categories.ToList();

            return View(viewModel);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var item = await _context.WarehouseItems
                .Include(w => w.Unit) // Načíst jednotku
                .Include(w => w.Categories) // Načíst kategorie
                .Include(w => w.Batches) // Načíst batch historie
                .FirstOrDefaultAsync(m => m.Id == id);

            if (item == null) return NotFound();

            return View(item);
        }

        public async Task<bool> ConsumeMaterial(int itemId, decimal quantity)
        {
            var batches = await _context.WarehouseItemBatches
                .Where(b => b.WarehouseItemId == itemId && b.Quantity > 0)
                .OrderBy(b => b.ExpirationDate)
                .ToListAsync();

            foreach (var batch in batches)
            {
                if (quantity <= 0) break;

                if (batch.Quantity >= quantity)
                {
                    batch.Quantity -= quantity;
                    quantity = 0;
                }
                else
                {
                    quantity -= batch.Quantity;
                    batch.Quantity = 0;
                }
            }

            await _context.SaveChangesAsync();
            return quantity == 0; // Vrátí true, pokud bylo vše pokryto
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            // Načteme položku ze skladu podle ID
            var warehouseItem = await _context.WarehouseItems
                .Include(w => w.Unit)
                .Include(w => w.Categories)
                .FirstOrDefaultAsync(w => w.Id == id);

            if (warehouseItem == null) return NotFound();

            // Vytvoříme viewModel s hodnotami pro formulář
            var viewModel = new WarehouseItemCreateViewModel
            {
                Name = warehouseItem.Name,
                Quantity = warehouseItem.Quantity,
                SelectedUnitId = warehouseItem.UnitId,
                PricePerUnit = warehouseItem.PricePerUnit,
                TaxPercentage = warehouseItem.TaxPercentage,
                MinQuantity = warehouseItem.MinQuantity,
                Supplier = warehouseItem.Supplier,
                Notes = warehouseItem.Notes,
                SelectedCategoryIds = warehouseItem.Categories.Select(c => c.Id).ToList(),
                AvailableUnits = await _context.Units.ToListAsync(),
                AvailableCategories = await _context.Categories.ToListAsync()
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, WarehouseItemCreateViewModel viewModel)
        {
            if (id != viewModel.Id) return NotFound();

            if (ModelState.IsValid)
            {
                var warehouseItem = await _context.WarehouseItems
                    .Include(w => w.Categories)
                    .FirstOrDefaultAsync(w => w.Id == id);

                if (warehouseItem == null) return NotFound();

                // Aktualizujeme položku podle nových hodnot
                warehouseItem.Name = viewModel.Name;
                warehouseItem.Quantity = viewModel.Quantity;
                warehouseItem.UnitId = viewModel.SelectedUnitId;
                warehouseItem.PricePerUnit = viewModel.PricePerUnit;
                warehouseItem.TaxPercentage = viewModel.TaxPercentage;
                warehouseItem.MinQuantity = viewModel.MinQuantity;
                warehouseItem.Supplier = viewModel.Supplier;
                warehouseItem.Notes = viewModel.Notes;

                // Aktualizujeme kategorie (vybereme pouze ty, které uživatel vybral)
                warehouseItem.Categories = _context.Categories
                    .Where(c => viewModel.SelectedCategoryIds.Contains(c.Id))
                    .ToList();

                _context.Update(warehouseItem); // Aktualizace položky
                await _context.SaveChangesAsync(); // Uložení změn

                return RedirectToAction(nameof(Index)); // Po úspěšné úpravě přejdeme zpět na seznam položek
            }

            // Pokud jsou nějaké chyby, znovu naplníme dostupné kategorie a jednotky
            viewModel.AvailableUnits = await _context.Units.ToListAsync();
            viewModel.AvailableCategories = await _context.Categories.ToListAsync();

            return View(viewModel); // Pokud model není validní, vrátíme uživatele zpět na stránku pro editaci
        }
    }
}