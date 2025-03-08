using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SoapProductionApp.Data;
using SoapProductionApp.Models.Cooking;
using SoapProductionApp.Models.Cooking.ViewModels;
using SoapProductionApp.Models.Recipe.ViewModels;

namespace SoapProductionApp.Controllers
{
    public class CookingController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CookingController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Cooking
        public async Task<IActionResult> Index()
        {
            var cookingList = await _context.Cookings
                .Include(c => c.Recipe)
                .ToListAsync();

            return View(cookingList);
        }

        // GET: Cooking/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var cooking = await _context.Cookings
                .Include(c => c.Recipe)
                .Include(c => c.UsedIngredients)
                .FirstOrDefaultAsync(c => c.Id == id);


            if (cooking == null) return NotFound();

            var viewModel = new CookingDetailViewModel
            {
                Id = cooking.Id,
                RecipeName = cooking.Recipe.Name,
                BatchSize = cooking.BatchSize,
                CookingDate = cooking.CookingDate,
                CuringDate = cooking.CuringDate,
                TotalCost = cooking.TotalCost,
                CostPerSoap = cooking.CostPerSoap,
                ExpirationDate = cooking.ExpirationDate,
                RecipeNotes = cooking.RecipeNotes,
                IsCut = cooking.IsCut,
                IsReadyToBeSold = cooking.IsReadyToBeSold,
                UsedIngredients = cooking.UsedIngredients.Select(i => new CookingIngredientViewModel
                {
                    IngredientName = i.IngredientName,
                    QuantityUsed = i.QuantityUsed,
                    Unit = i.Unit,
                    Cost = (i.QuantityUsed * _context.WarehouseItems
                        .FirstOrDefault(w => w.Name == i.IngredientName)?.AveragePricePerUnitWithoutTax) ?? 0,
                    ExpirationDate = i.ExpirationDate
                }).ToList()
            };


            return View(viewModel);
        }

        // GET: Cooking/Create
        public async Task<IActionResult> Create(int? selectedRecipeId)
        {
            var model = new CookingCreateViewModel();
            model.Recipes = await _context.Recipes.ToListAsync();

            // Pokud uživatel v dropdownu nějaký recept vybral (a odeslal GET formulář)
            if (selectedRecipeId.HasValue)
            {
                var recipe = await _context.Recipes
                    .Include(r => r.Ingredients)
                    .ThenInclude(i => i.WarehouseItem)
                    .FirstOrDefaultAsync(r => r.Id == selectedRecipeId.Value);

                if (recipe != null)
                {
                    // Uložíme si do modelu ID vybraného receptu
                    model.SelectedRecipeId = recipe.Id;
                    // Předvyplníme BatchSize a RecipeNotes
                    model.BatchSize = recipe.BatchSize;
                    model.RecipeNotes = recipe.Note;

                    // Připravíme si seznam ingrediencí k zobrazení
                    model.Ingredients = recipe.Ingredients
                        .Select(i => new RecipeIngredientViewModel
                        {
                            WarehouseItemName = i.WarehouseItem.Name,
                            Quantity = i.Quantity,
                            Unit = i.Unit.ToString()
                        })
                        .ToList();
                }
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int selectedRecipeId, int batchSize, string? recipeNotes)
        {
            // Najdeme recept podle ID
            var recipe = await _context.Recipes
                .Include(r => r.Ingredients)
                .ThenInclude(i => i.WarehouseItem)
                .ThenInclude(w => w.Batches)
                .FirstOrDefaultAsync(r => r.Id == selectedRecipeId);

            if (recipe == null) return NotFound();

            // Vytvoříme nový Cooking
            var cooking = new Cooking
            {
                RecipeId = recipe.Id,
                Recipe = recipe,
                BatchSize = batchSize,
                CookingDate = DateTime.Now,
                CuringDate = DateTime.Now.AddDays(recipe.DaysOfCure),
                RecipeNotes = recipeNotes,
                IsCut = false,
                IsReadyToBeSold = false,
                UsedIngredients = new List<CookingIngredient>()
            };

            // Dále zpracování surovin/batches a výpočet costu podle vaší logiky:
            decimal totalCost = 0;
            foreach (var ingredient in recipe.Ingredients)
            {
                decimal requiredAmount = ingredient.Quantity;
                decimal takenAmount = 0;

                var batches = ingredient.WarehouseItem.Batches
                    .Where(b => b.AvailableQuantity > 0)
                    .OrderBy(b => b.ExpirationDate)
                    .ToList();

                foreach (var batch in batches)
                {
                    if (takenAmount >= requiredAmount) break;

                    decimal takeFromBatch = Math.Min(batch.AvailableQuantity, requiredAmount - takenAmount);
                    batch.AvailableQuantity -= takeFromBatch;
                    takenAmount += takeFromBatch;

                    var cookingIngredient = new CookingIngredient
                    {
                        IngredientName = ingredient.WarehouseItem.Name,
                        QuantityUsed = takeFromBatch,
                        Unit = ingredient.Unit.ToString(),
                        ExpirationDate = batch.ExpirationDate ?? DateTime.Now.AddYears(1)
                    };

                    cooking.UsedIngredients.Add(cookingIngredient);
                    totalCost += takeFromBatch * batch.UnitPriceWithoutTax;
                }
            }
            _context.Cookings.Add(cooking);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: Cooking/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var cooking = await _context.Cookings
                .Include(r => r.Recipe)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (cooking == null) return NotFound();
            return View(cooking);
        }

        // POST: Cooking/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Cooking cooking)
        {
            if (id != cooking.Id) return NotFound();

            _context.Update(cooking);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Cooking/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var cooking = await _context.Cookings
                .Include(c => c.Recipe)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (cooking == null) return NotFound();

            return View(cooking);
        }

        // POST: Cooking/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var cooking = await _context.Cookings.FindAsync(id);
            if (cooking != null)
            {
                _context.Cookings.Remove(cooking);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
