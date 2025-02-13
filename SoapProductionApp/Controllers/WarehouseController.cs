using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SoapProductionApp.Data;
using SoapProductionApp.Models;
using SoapProductionApp.Models.Warehouse;
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

        public async Task<IActionResult> Index()
        {
            var items = await _context.WarehouseItems
                .Include(i => i.Batches)
                .Include(i => i.Categories)
                .ToListAsync();
            return View(items);
        }

        public async Task<IActionResult> Create()
        {
            var viewModel = new WarehouseItemCreateViewModel
            {
                AvailableCategories = _context.Categories.ToList(),
                AvailableUnits = Enum.GetValues(typeof(UnitMeasurement.UnitType)).Cast<UnitMeasurement.UnitType>().ToList()
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
                    Unit = viewModel.Unit,
                    DefaultUnit = viewModel.DefaultUnit,
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

            viewModel.AvailableCategories = await _context.Categories.ToListAsync();
            viewModel.AvailableUnits = Enum.GetValues(typeof(UnitMeasurement.UnitType)).Cast<UnitMeasurement.UnitType>().ToList();

            return View(viewModel);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var warehouseItem = await _context.WarehouseItems
                .Include(w => w.Categories)
                .Include(w => w.Batches)
                .FirstOrDefaultAsync(w => w.Id == id);

            if (warehouseItem == null) return NotFound();

            var viewModel = new WarehouseItemCreateViewModel(warehouseItem);

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

                warehouseItem.Name = viewModel.Name;
                warehouseItem.Unit = viewModel.Unit;
                warehouseItem.DefaultUnit = viewModel.DefaultUnit;
                warehouseItem.TaxPercentage = viewModel.TaxPercentage;
                warehouseItem.MinQuantity = viewModel.MinQuantity;
                warehouseItem.Supplier = viewModel.Supplier;
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
    }
}