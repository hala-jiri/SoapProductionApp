using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SoapProductionApp.Data;
using SoapProductionApp.Models.RecipeModels; // To musí být na začátku!

namespace SoapProductionApp.Controllers
{
    public class RecipeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RecipeController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Zobrazení seznamu receptů
        public async Task<IActionResult> Index()
        {
            var recipes = await _context.Recipes
                            .Include(r => r.Ingredients)
                            .ThenInclude(ri => ri.WarehouseItem)
                            .ToListAsync();

            var recipeList = recipes.Select(recipe => new RecipeOverviewViewModel
            {
                Id = recipe.Id,
                Name = recipe.Name,
                BatchSize = recipe.BatchSize,
                TotalCost = recipe.Ingredients.Sum(i => i.Quantity * i.WarehouseItem.PricePerUnit),
                CostPerPiece = recipe.Ingredients.Sum(i => i.Quantity * i.WarehouseItem.PricePerUnit) / recipe.BatchSize,
                HasEnoughStock = recipe.Ingredients.All(i => i.WarehouseItem.Quantity >= i.Quantity * recipe.BatchSize)
            }).ToList();

            return View(recipeList);
        }

        // GET: Recipe/Create
        public IActionResult Create()
        {
            ViewBag.WarehouseItems = _context.WarehouseItems
                .Select(w => new
                {
                    Id = w.Id,
                    Name = w.Name,
                    UnitName = w.Unit.Name,
                    PricePerUnit = w.PricePerUnit
                })
                .ToList();

            return View(new Recipe());
        }

        // POST: Recipe/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Recipe recipe)
        {
            if (ModelState.IsValid)
            {
                _context.Recipes.Add(recipe);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // Opětovné naplnění ViewBag při chybě ve formuláři
            ViewBag.WarehouseItems = _context.WarehouseItems
                .Select(w => new
                {
                    Id = w.Id,
                    Name = w.Name,
                    UnitName = w.Unit.Name,
                    PricePerUnit = w.PricePerUnit
                })
                .ToList();

            return View(recipe);
        }

        // Zobrazení detailu receptu
        public async Task<IActionResult> Details(int id)
        {
            var recipe = await _context.Recipes
                .Include(r => r.Ingredients)
                .ThenInclude(i => i.WarehouseItem)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (recipe == null) return NotFound();
            return View(recipe);
        }

        // GET: Recipe/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var recipe = await _context.Recipes
                .Include(r => r.Ingredients)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (recipe == null)
            {
                return NotFound();
            }

            return View(recipe);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Recipe recipe)
        {
            if (id != recipe.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(recipe);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Recipes.Any(e => e.Id == id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            return View(recipe);
        }
    }
}
