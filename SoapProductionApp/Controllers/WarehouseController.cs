using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Elfie.Serialization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SoapProductionApp.Data;
using SoapProductionApp.Models;
using SoapProductionApp.Models.Warehouse;
using SoapProductionApp.Models.Warehouse.ViewModels;
using System.Linq;
using System.Threading.Tasks;

namespace SoapProductionApp.Controllers
{
    public class WarehouseController : Controller
    {
        private readonly ApplicationDbContext _context;

        public WarehouseController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string sortOrder, string searchString, List<int> categoryIds)
        {
            ViewData["NameSortParam"] = string.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["SearchString"] = searchString;

            // Načteme všechny dostupné kategorie pro zobrazení ve filtru
            var categories = await _context.Categories.OrderBy(c => c.Name).ToListAsync();
            ViewData["Categories"] = categories;

            var warehouseItems = _context.WarehouseItems
                .Include(i => i.Batches)
                .Include(i => i.Categories)
                .AsQueryable();

            // Filtrování podle názvu (pokud existuje searchString)
            if (!string.IsNullOrEmpty(searchString))
                warehouseItems = warehouseItems.Where(w => w.Name.Contains(searchString));

            // Filtrování podle kategorií (pokud jsou vybrané nějaké kategorie)
            if (categoryIds != null && categoryIds.Any())
                warehouseItems = warehouseItems.Where(w => w.Categories.Any(c => categoryIds.Contains(c.Id)));

            // Řazení podle názvu (ASC nebo DESC)
            warehouseItems = sortOrder == "name_desc"
                ? warehouseItems.OrderByDescending(i => i.Name)
                : warehouseItems.OrderBy(i => i.Name);
            
            return View(await warehouseItems.ToListAsync());
        }

        public async Task<IActionResult> Create()
        {
            // Načteme všechny dostupné kategorie
            var availableCategories = await _context.Categories.ToListAsync();

            // Vytvoříme ViewModel
            var viewModel = new WarehouseItemCreateEditViewModel
            {
                AvailableUnits = Enum.GetValues(typeof(UnitMeasurement.UnitType)).Cast<UnitMeasurement.UnitType>().ToList(),
                AvailableCategories = availableCategories // Všechny kategorie
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(WarehouseItemCreateEditViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var warehouseItem = new WarehouseItem
                {
                    Name = viewModel.Name,
                    Unit = viewModel.Unit,
                    TaxPercentage = viewModel.TaxPercentage,
                    MinimumQuantityAlarm = viewModel.MinimumQuantityAlarm,
                    Notes = viewModel.Notes,
                    Categories = _context.Categories.Where(c => viewModel.SelectedCategoryIds.Contains(c.Id)).ToList()
                };

                _context.WarehouseItems.Add(warehouseItem);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            viewModel.AvailableCategories = await _context.Categories.ToListAsync();
            viewModel.AvailableUnits = Enum.GetValues(typeof(UnitMeasurement.UnitType)).Cast<UnitMeasurement.UnitType>().ToList();

            return View(viewModel);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var warehouseItem = await _context.WarehouseItems
                .Include(w => w.Categories)// Načteme existující kategorie přiřazené k WarehouseItem
                .Include(w => w.Batches)
                .FirstOrDefaultAsync(w => w.Id == id);

            if (warehouseItem == null) return NotFound();

            // Načteme všechny dostupné kategorie
            var availableCategories = await _context.Categories.ToListAsync();

            var viewModel = new WarehouseItemCreateEditViewModel();
            viewModel.Id = warehouseItem.Id;
            viewModel.Name = warehouseItem.Name;
            viewModel.AvailableCategories = availableCategories;
            viewModel.AvailableUnits = Enum.GetValues(typeof(UnitMeasurement.UnitType)).Cast<UnitMeasurement.UnitType>().ToList();

            viewModel.SelectedCategoryIds = warehouseItem.Categories.Select(x => x.Id).ToList();
            viewModel.TaxPercentage = warehouseItem.TaxPercentage;
            viewModel.MinimumQuantityAlarm = warehouseItem.MinimumQuantityAlarm;
            viewModel.Unit = warehouseItem.Unit;
            viewModel.Notes = warehouseItem.Notes;

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, WarehouseItemCreateEditViewModel viewModel)
        {
            if (id != viewModel.Id) return NotFound();

            var warehouseItem = await _context.WarehouseItems
                .Include(w => w.Categories)
                .Include(w => w.Batches)
                .FirstOrDefaultAsync(w => w.Id == id);

            if (warehouseItem == null) return NotFound();

            if (ModelState.IsValid)
            {
                warehouseItem.Name = viewModel.Name;
                warehouseItem.Unit = viewModel.Unit;
                warehouseItem.TaxPercentage = viewModel.TaxPercentage;
                warehouseItem.MinimumQuantityAlarm = viewModel.MinimumQuantityAlarm;
                warehouseItem.Notes = viewModel.Notes;
                warehouseItem.Categories = _context.Categories
                    .Where(c => viewModel.SelectedCategoryIds.Contains(c.Id))
                    .ToList();

                _context.Update(warehouseItem);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            viewModel.AvailableCategories = await _context.Categories.ToListAsync();
            viewModel.AvailableUnits = Enum.GetValues(typeof(UnitMeasurement.UnitType)).Cast<UnitMeasurement.UnitType>().ToList();
            
            return View(viewModel);
        }

        public async Task<IActionResult> Details(int id)
        {
            //TODO: here I need to load warehouseItem and convert it to WarehouseItemViewModel
            // Also add some batchViewModels in that item

            var warehouseItem = await _context.WarehouseItems
                .Include(w => w.Batches)
                .Include(w => w.Categories)
                .FirstOrDefaultAsync(w => w.Id == id);

            if (warehouseItem == null) return NotFound();


            return View(warehouseItem);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var warehouseItem = await _context.WarehouseItems
                .Include(w => w.Categories) // Načteme přiřazené kategorie (pro případné zobrazení)
                .FirstOrDefaultAsync(w => w.Id == id);

            if (warehouseItem == null) return NotFound();

            return View(warehouseItem);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var warehouseItem = await _context.WarehouseItems
                .Include(w => w.Categories)
                .Include(w => w.Batches)
                .FirstOrDefaultAsync(w => w.Id == id);

            if (warehouseItem == null) return NotFound();

            // Odstraníme kategorie a šarže
            warehouseItem.Categories.Clear();
            _context.Batches.RemoveRange(warehouseItem.Batches);

            // Odstraníme WarehouseItem
            _context.WarehouseItems.Remove(warehouseItem);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}