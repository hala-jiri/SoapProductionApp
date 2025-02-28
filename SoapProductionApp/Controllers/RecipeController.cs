using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SoapProductionApp.Data;
using SoapProductionApp.Models;
using SoapProductionApp.Models.Recipe;
using SoapProductionApp.Models.Recipe.ViewModels;

namespace SoapProductionApp.Controllers
{
    public class RecipeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RecipeController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Zobrazí seznam receptů
        public async Task<IActionResult> Index()
        {
            var recipes = await _context.Recipes
                .Include(r => r.Ingredients)
                .ThenInclude(ri => ri.WarehouseItem)
                .ThenInclude(b => b.Batches).ToListAsync();
            return View(recipes);
        }

        // Detail receptu
        public async Task<IActionResult> Details(int id)
        {
            var recipe = await _context.Recipes
                .Include(r => r.Ingredients)
                .ThenInclude(ri => ri.WarehouseItem)
                .ThenInclude(b => b.Batches)
                .FirstOrDefaultAsync(r => r.Id == id);

            //var fakeRecipe = getFakeData();

            if (recipe == null)
                return NotFound();

            return View(recipe);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var model = new RecipeCreateEditViewModel
            {
                AvailableWarehouseItems = await _context.WarehouseItems.Include(x=> x.Batches).ToListAsync()
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(RecipeCreateEditViewModel model)
        { //nefunguje spravne! not getting "Ingredients in model!
            ModelState.Remove("ImageUrl");
            ModelState.Remove("AvailableWarehouseItems");
            if (ModelState.IsValid)
            {
                var recipe = new Recipe(model);

                var ingredients = model.Ingredients.Select(i => new RecipeIngredient
                {
                    WarehouseItemId = i.WarehouseItemId,
                    Quantity = Math.Round(i.Quantity, 2), // Round Quantity to 2 decimal places
                    Unit = Enum.TryParse<UnitMeasurement.UnitType>(i.Unit, out var unitEnum)
                        ? unitEnum
                        : throw new InvalidOperationException($"Invalid unit type: {i.Unit}")
                }).ToList();

                foreach (var ingredience in ingredients)
                {
                    var warehouseItem = await _context.WarehouseItems
                        .Include(w => w.Batches)
                    .FirstOrDefaultAsync(w => w.Id == ingredience.WarehouseItemId);
                    ingredience.WarehouseItem = warehouseItem;
                }

                recipe.Ingredients = ingredients;
                /*{
                    Name = model.Name,
                    ImageUrl = model.ImageUrl,
                    BatchSize = model.BatchSize,
                    DaysOfCure = model.DaysOfCure,
                    Ingredients = model.Ingredients.Select(i => new RecipeIngredient
                    {
                        WarehouseItemId = i.WarehouseItemId,
                        Quantity = i.Quantity,
                        Unit = Enum.TryParse<UnitMeasurement.UnitType>(i.Unit, out var unitEnum)
                               ? unitEnum
                               : throw new InvalidOperationException($"Invalid unit type: {i.Unit}")
                    }).ToList()
                };*/

                _context.Recipes.Add(recipe);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            model.AvailableWarehouseItems = await _context.WarehouseItems.ToListAsync();
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var recipe = await _context.Recipes
                .Include(r => r.Ingredients)
                .ThenInclude(i => i.WarehouseItem)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (recipe == null)
            {
                return NotFound();
            }

            var viewModel = new RecipeCreateEditViewModel
            {
                Id = recipe.Id,
                Name = recipe.Name,
                BatchSize = recipe.BatchSize,
                DaysOfCure = recipe.DaysOfCure,
                Note = recipe.Note,
                Ingredients = recipe.Ingredients.Select(i => new RecipeIngredientViewModel
                {
                    WarehouseItemId = i.WarehouseItemId,
                    WarehouseItemName = i.WarehouseItem.Name,
                    Quantity = i.Quantity,
                    Unit = i.Unit.ToString(),
                    Cost = i.CostPerIngredient
                }).ToList()
            };

            return View(viewModel);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var recipe = await _context.Recipes.FindAsync(id);
            if (recipe != null)
            {
                _context.Recipes.Remove(recipe);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var recipe = await _context.Recipes
                .Include(r => r.Ingredients)
                .ThenInclude(i => i.WarehouseItem)
                .ThenInclude(b => b.Batches)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (recipe == null) return NotFound();

            var model = new RecipeCreateEditViewModel
            {
                Id = recipe.Id,
                Name = recipe.Name,
                Note = recipe.Note,
                ImageUrl = recipe.ImageUrl,
                BatchSize = recipe.BatchSize,
                DaysOfCure = recipe.DaysOfCure,
                Ingredients = recipe.Ingredients.Select(i => new RecipeIngredientViewModel
                {
                    WarehouseItemId = i.WarehouseItemId,
                    Quantity = i.Quantity,
                    Unit = i.Unit.ToString(),
                    Cost = i.CostPerIngredient
                }).ToList(),
                AvailableWarehouseItems = await _context.WarehouseItems.Include(x => x.Batches).ToListAsync()
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(RecipeCreateEditViewModel model)
        {
            ModelState.Remove("AvailableWarehouseItems");
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                Console.WriteLine("ModelState Errors: " + string.Join(", ", errors));
                model.AvailableWarehouseItems = await _context.WarehouseItems.Include(x => x.Batches).ToListAsync();
                return View(model);
            }


            if (ModelState.IsValid)
            {
                var recipe = await _context.Recipes
                    .Include(r => r.Ingredients)
                    .FirstOrDefaultAsync(r => r.Id == model.Id);

                if (recipe == null) return NotFound();

                recipe.Name = model.Name;
                recipe.Note = model.Note;   
                recipe.ImageUrl = model.ImageUrl;
                recipe.BatchSize = model.BatchSize;
                recipe.DaysOfCure = model.DaysOfCure;

                // Aktualizace ingrediencí
                recipe.Ingredients.Clear();
                foreach (var ingredient in model.Ingredients)
                {
                    recipe.Ingredients.Add(new RecipeIngredient
                    {
                        WarehouseItemId = ingredient.WarehouseItemId,
                        Quantity = ingredient.Quantity,
                        Unit = Enum.Parse<UnitMeasurement.UnitType>(ingredient.Unit)
                    });
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            model.AvailableWarehouseItems = await _context.WarehouseItems.ToListAsync();
            return View(model);
        }

        public Recipe getFakeData()
        {
            //TEST
            var oliveOil = _context.WarehouseItems
                 .Include(x => x.Batches) // Batches je navigační vlastnost (List<Batch>)
                 .FirstOrDefault(x => x.Id == 14);
            var citrus = _context.WarehouseItems
                 .Include(x => x.Batches) // Batches je navigační vlastnost (List<Batch>)
                 .FirstOrDefault(x => x.Id == 15);

            var recipeIngredience1 = new RecipeIngredient()
            {
                Id = 1,
                WarehouseItemId = oliveOil.Id,
                Quantity = (decimal)0.75,
                Unit = oliveOil.Unit,
                WarehouseItem = oliveOil
            };
            var recipeIngredience2 = new RecipeIngredient()
            {
                Id = 2,
                WarehouseItemId = citrus.Id,
                Quantity = (decimal)2,
                Unit = citrus.Unit,
                WarehouseItem = citrus
            };

            var recipeFake = new Recipe()
            {
                BatchSize = 11,
                DaysOfCure = 60,
                Id = 5,
                Name = "my test recipe",
                Ingredients = new List<RecipeIngredient>() { recipeIngredience1, recipeIngredience2 }
            };
            return recipeFake;
        }
        /*// Zobrazení seznamu receptů
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
                    UnitName = w.Unit.ToString(),
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
                // Ručně nastavíme WarehouseItem podle ID
                foreach (var ingredient in recipe.Ingredients)
                {
                    ingredient.WarehouseItem = await _context.WarehouseItems.FindAsync(ingredient.WarehouseItemId);
                    ingredient.Recipe = recipe; // Přiřadíme recept
                }

                _context.Recipes.Add(recipe);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // Při chybě znovu načteme ViewBag s ingrediencemi
            ViewBag.WarehouseItems = _context.WarehouseItems
                .Select(w => new
                {
                    Id = w.Id,
                    Name = w.Name,
                    UnitName = w.Unit.ToString(),
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
        }*/
    }
}
