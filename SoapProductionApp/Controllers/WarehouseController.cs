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
    }
}