using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SoapProductionApp.Data;
using SoapProductionApp.Models;
using SoapProductionApp.Models.Recipe;
using SoapProductionApp.Models.Recipe.ViewModels;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Formats;

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
        {
            ModelState.Remove("ImageUrl");
            ModelState.Remove("AvailableWarehouseItems");
            if (ModelState.IsValid)
            {
                var ingredients = model.Ingredients.Select(i => new RecipeIngredient
                {
                    WarehouseItemId = i.WarehouseItemId,
                    Quantity = Math.Round(i.Quantity, 2),
                    Unit = Enum.TryParse<UnitMeasurement.UnitType>(i.Unit, out var unitEnum)
                        ? unitEnum
                        : throw new InvalidOperationException($"Invalid unit type: {i.Unit}")
                }).ToList();

                foreach (var ingredience in ingredients)
                {
                    var warehouseItem = await _context.WarehouseItems
                        .Include(w => w.Batches)
                    .FirstOrDefaultAsync(w => w.Id == ingredience.WarehouseItemId);
                    if (warehouseItem != null)
                        ingredience.WarehouseItem = warehouseItem;
                }
                var recipe = new Recipe(model, ingredients);
                recipe.Ingredients = ingredients;

                if (model.ImageFile != null)
                {
                    var (imagePath, thumbPath) = await SaveImageAsync(model.ImageFile);
                    recipe.ImageUrl = imagePath;
                    recipe.ThumbnailUrl = thumbPath;
                }

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
                .ThenInclude(b => b.Batches)
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
                ProductType = recipe.ProductType,
                ImageUrl = recipe.ImageUrl,
                ThumbnailUrl = recipe.ThumbnailUrl,
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
                DeleteImage(recipe.ImageUrl, recipe.ThumbnailUrl);
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
                ThumbnailUrl = recipe.ThumbnailUrl,
                BatchSize = recipe.BatchSize,
                DaysOfCure = recipe.DaysOfCure,
                ProductType = recipe.ProductType,
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
            foreach (var ing in model.Ingredients) //Sadly need to remove ItemName as I cannot fill it in view :/
            {
                ModelState.Remove($"Ingredients[{model.Ingredients.IndexOf(ing)}].WarehouseItemName");
            }

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
                recipe.BatchSize = model.BatchSize;
                recipe.DaysOfCure = model.DaysOfCure;
                recipe.ProductType = model.ProductType;

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

                // Pokud uživatel nahrál nový obrázek, smažeme starý
                if (model.ImageFile != null)
                {
                    DeleteImage(recipe.ImageUrl, recipe.ThumbnailUrl);
                    var (imagePath, thumbPath) = await SaveImageAsync(model.ImageFile);
                    recipe.ImageUrl = imagePath;
                    recipe.ThumbnailUrl = thumbPath;
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            model.AvailableWarehouseItems = await _context.WarehouseItems.ToListAsync();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteImage(int id)
        {
            var recipe = await _context.Recipes.FindAsync(id);
            if (recipe == null) return NotFound();

            DeleteImage(recipe.ImageUrl, recipe.ThumbnailUrl);

            // Odstraníme URL z databáze
            recipe.ImageUrl = null;
            recipe.ThumbnailUrl = null;
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Edit), new { id });
        }

        private async Task<(string ImagePath, string ThumbPath)> SaveImageAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return (null, null);

            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
            var thumbnailsFolder = Path.Combine(uploadsFolder, "thumbnails");

            Directory.CreateDirectory(uploadsFolder);
            Directory.CreateDirectory(thumbnailsFolder);

            // generate name
            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var filePath = Path.Combine(uploadsFolder, fileName);
            var thumbPath = Path.Combine(thumbnailsFolder, fileName);

            // save of main pic
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // create miniature
            using (var image = Image.Load(file.OpenReadStream()))
            {
                image.Mutate(x => x.Resize(40, 40));
                image.Save(thumbPath);
            }

            return ($"/uploads/{fileName}", $"/uploads/thumbnails/{fileName}");
        }

        private void DeleteImage(string imageUrl, string thumbUrl)
        {
            if (!string.IsNullOrEmpty(imageUrl))
            {
                var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", imageUrl.TrimStart('/'));
                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }
            }

            if (!string.IsNullOrEmpty(thumbUrl))
            {
                var thumbPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", thumbUrl.TrimStart('/'));
                if (System.IO.File.Exists(thumbPath))
                {
                    System.IO.File.Delete(thumbPath);
                }
            }
        }
    }
}
