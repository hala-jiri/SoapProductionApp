using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SoapProductionApp.Data;
using SoapProductionApp.Models.ProductUnit.ViewModels;

namespace SoapProductionApp.Controllers
{
    public class ProductUnitsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProductUnitsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var units = await _context.Cookings
                .Include(c => c.ProductUnits)
                .Include(c => c.Recipe)
                .Include(c => c.UsedIngredients)
                .Where(c => c.IsCut)
                .Select(c => new ProductUnitOverviewViewModel
                {
                    CookingId = c.Id,
                    RecipeName = c.Recipe.Name,
                    ProductType = c.Recipe.ProductType,
                    ExpirationDate = c.ExpirationDate ?? c.CuringDate.AddMonths(12),
                    Cost = c.CostPerSoap,
                    TotalUnits = c.ProductUnits.Count,
                    UnsoldUnits = c.ProductUnits.Count(p => !p.IsSold)
                })
                .ToListAsync();

            return View(units);
        }

        public async Task<IActionResult> Details(int cookingId)
        {
            var cooking = await _context.Cookings
                .Include(c => c.Recipe)
                .Include(c => c.ProductUnits)
                .FirstOrDefaultAsync(c => c.Id == cookingId);

            if (cooking == null) return NotFound();

            var viewModel = new ProductUnitDetailViewModel
            {
                CookingId = cooking.Id,
                RecipeName = cooking.Recipe.Name,
                ProductType = cooking.Recipe.ProductType,
                TotalUnits = cooking.ProductUnits.Count,
                SoldUnits = cooking.ProductUnits.Count(p => p.IsSold),
                UnsoldUnits = cooking.ProductUnits.Count(p => !p.IsSold),
                Units = cooking.ProductUnits
                    .OrderBy(p => p.IsSold)
                    .ThenBy(p => p.Id)
                    .ToList()
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Sell(int cookingId, int count)
        {
            var units = await _context.ProductUnits
                .Where(p => p.CookingId == cookingId && !p.IsSold)
                .OrderBy(p => p.Id)
                .Take(count)
                .ToListAsync();

            foreach (var unit in units)
            {
                unit.IsSold = true;
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Details", new { cookingId });
        }
    }
}
