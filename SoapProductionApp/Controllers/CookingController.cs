using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SoapProductionApp.Data;
using SoapProductionApp.Models.Cooking;
using SoapProductionApp.Models.Cooking.ViewModels;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

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
                .Include(c => c.UsedIngredients)
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
                BatchSizeWasChanged = cooking.BatchSizeWasChanged,
                CookingDate = cooking.CookingDate,
                CuringDate = cooking.CuringDate,
                RecipeProductType = cooking.Recipe.ProductType,
                TotalCost = cooking.TotalCost,
                ExpirationDate = cooking.ExpirationDate,
                RecipeNotes = cooking.RecipeNotes,
                CookingNotes = cooking.CookingNotes,
                ImageUrl = cooking.Recipe.ImageUrl,
                ThumbnailUrl = cooking.Recipe.ThumbnailUrl,
                IsCut = cooking.IsCut,
                UsedIngredients = cooking.UsedIngredients.Select(i => new CookingIngredientViewModel
                {
                    IngredientName = i.IngredientName,
                    QuantityUsed = i.QuantityUsed,
                    Unit = i.Unit,
                    Cost = i.Cost,
                    ExpirationDate = i.ExpirationDate
                }).ToList()
            };


            return View(viewModel);
        }

        // GET: Cooking/Create
        public async Task<IActionResult> Create(int? selectedRecipeId)
        {
            var model = new CookingCreateViewModel();
            model.Recipes = await _context.Recipes
                    .Include(r => r.Ingredients)
                    .ThenInclude(i => i.WarehouseItem)
                    .ThenInclude(i => i.Batches)
                    .ToListAsync();

            // Pokud uživatel v dropdownu nějaký recept vybral (a odeslal GET formulář)
            if (selectedRecipeId.HasValue)
            {
                var recipe = await _context.Recipes
                    .Include(r => r.Ingredients)
                    .ThenInclude(i => i.WarehouseItem)
                    .ThenInclude(i => i.Batches)
                    .FirstOrDefaultAsync(r => r.Id == selectedRecipeId.Value);

                if (recipe != null)
                {

                    // Uložíme si do modelu ID vybraného receptu
                    model.SelectedRecipeId = recipe.Id;

                    // Předvyplníme BatchSize a RecipeNotes
                    model.ProductType = recipe.ProductType;
                    model.BatchSize = recipe.BatchSize;
                    model.RecipeNotes = recipe.Note;
                    model.ImageUrl = recipe.ImageUrl;
                    model.ThumbnailUrl = recipe.ThumbnailUrl;
                    model.TotalCost = recipe.CostPerBatch;
                    // Připravíme si seznam ingrediencí k zobrazení
                    model.UsedIngredients = recipe.Ingredients
                        .Select(i => new CookingIngredientViewModel
                        {
                            IngredientName = i.WarehouseItem.Name,
                            QuantityUsed = i.Quantity,
                            Unit = i.Unit.ToString(),
                            ExpirationDate = i.NearestExpirationDate.Value
                        })
                        .ToList();
                }
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int selectedRecipeId, int batchSize, string? cookingNotes)
        {
            // Najdeme recept podle ID
            var recipe = await _context.Recipes
                .Include(r => r.Ingredients)
                .ThenInclude(i => i.WarehouseItem)
                .ThenInclude(w => w.Batches)
                .FirstOrDefaultAsync(r => r.Id == selectedRecipeId);

            if (recipe == null) return NotFound();

            // need to check stock value
            if(!recipe.AreAllIngredientsInStock)
                return RedirectToAction(nameof(Index));

            // Vytvoříme nový Cooking
            var cooking = new Cooking
            {
                RecipeId = recipe.Id,
                Recipe = recipe,
                BatchSize = batchSize,
                CookingDate = DateTime.Now,
                CuringDate = DateTime.Now.AddDays(recipe.DaysOfCure),
                RecipeNotes = recipe.Note,
                CookingNotes = cookingNotes,
                IsCut = false,
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
                        ExpirationDate = batch.ExpirationDate ?? DateTime.Now.AddYears(1),
                        Cost = takeFromBatch * batch.UnitPriceWithoutTax
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

            var cookingOldRecord = await _context.Cookings
                .Include(c => c.Recipe)
                .Include(c => c.UsedIngredients)
                .FirstOrDefaultAsync(c => c.Id == cooking.Id);

            if (cookingOldRecord == null) return NotFound();

            cookingOldRecord.CookingNotes = cooking.CookingNotes;

            _context.Update(cookingOldRecord);
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


        public async Task<ActionResult> ExportToPdf(int id)
        {
            // Nastavení Community License pro QuestPDF
            QuestPDF.Settings.License = LicenseType.Community;

            var cooking = await _context.Cookings
                .Include(c => c.Recipe)
                .Include(c => c.UsedIngredients)
                .FirstOrDefaultAsync(c => c.Id == id);


            if (cooking == null) return NotFound();

            var cookingDetailViewModel = new CookingDetailViewModel
            {
                Id = cooking.Id,
                RecipeName = cooking.Recipe.Name,
                BatchSize = cooking.BatchSize,
                BatchSizeWasChanged = cooking.BatchSizeWasChanged,
                CookingDate = cooking.CookingDate,
                RecipeProductType = cooking.Recipe.ProductType,
                CuringDate = cooking.CuringDate,
                TotalCost = cooking.TotalCost,
                ExpirationDate = cooking.ExpirationDate,
                RecipeNotes = cooking.RecipeNotes,
                CookingNotes = cooking.CookingNotes,
                IsCut = cooking.IsCut,
                UsedIngredients = cooking.UsedIngredients.Select(i => new CookingIngredientViewModel
                {
                    IngredientName = i.IngredientName,
                    QuantityUsed = i.QuantityUsed,
                    Unit = i.Unit,
                    Cost = i.Cost,
                    ExpirationDate = i.ExpirationDate
                }).ToList()
            };


            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(30);
                    page.Background(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(12));

                    page.Header()
                        .Background("#17a2b8") // Modrá hlavička jako v HTML
                        .Padding(10)
                        .AlignCenter()
                        .Text("Cooking Details")
                        .FontSize(20).Bold().FontColor(Colors.White);

                    page.Content()
                        .Column(col =>
                        {
                            col.Item().PaddingBottom(10); // ✅ Přidává mezeru mezi nadpisem a "Recipe Name"

                            col.Item().Row(row =>
                            {
                                row.RelativeItem().Text("Recipe Name:").SemiBold();
                                row.RelativeItem().AlignRight().Text(cookingDetailViewModel.RecipeName);
                            });

                            col.Item().Row(row =>
                            {
                                row.RelativeItem().Text("Product Type:").SemiBold();
                                row.RelativeItem().AlignRight().Text(cookingDetailViewModel.RecipeProductType.ToString());
                            });

                            col.Item().Row(row =>
                            {
                                row.RelativeItem().Text("Batch Size:").SemiBold();
                                row.RelativeItem().AlignRight().Text(txt =>
                                {
                                    txt.Span($"{cookingDetailViewModel.BatchSize} pcs").NormalWeight();

                                    if (cookingDetailViewModel.BatchSizeWasChanged)
                                    {
                                        txt.Span(" (Modified)").FontSize(10).FontColor("#FF0000").Italic();
                                    }
                                });
                            });

                            col.Item().Row(row =>
                            {
                                row.RelativeItem().Text("Cooking Date:").SemiBold();
                                row.RelativeItem().AlignRight().Text(cookingDetailViewModel.CookingDate.ToString("dd/MM/yyyy"));
                            });

                            col.Item().Row(row =>
                            {
                                row.RelativeItem().Text("Curing Date:").SemiBold();
                                row.RelativeItem().AlignRight().Text(cookingDetailViewModel.CuringDate.ToString("dd/MM/yyyy"));
                            });

                            col.Item().Row(row =>
                            {
                                row.RelativeItem().Text("Expiration Date:").SemiBold();
                                row.RelativeItem().AlignRight().Text(cookingDetailViewModel.ExpirationDate.HasValue ? cookingDetailViewModel.ExpirationDate.Value.ToString("dd/MM/yyyy") : "N/A");
                            });

                            col.Item().Row(row =>
                            {
                                row.RelativeItem().Text("Total Cost:").SemiBold();
                                row.RelativeItem().AlignRight().Text($"{cookingDetailViewModel.TotalCost:0.00} €");
                            });

                            col.Item().Row(row =>
                            {
                                row.RelativeItem().Text("Cost per Soap:").SemiBold();
                                row.RelativeItem().AlignRight().Text($"{cookingDetailViewModel.CostPerSoap:0.00} €");
                            });

                            col.Item().PaddingTop(10).Text("Recipe Notes:").Bold();
                            col.Item().Text(cookingDetailViewModel.RecipeNotes).Italic();

                            col.Item().PaddingTop(10).Text("Cooking Notes:").Bold();
                            col.Item().Text(cookingDetailViewModel.CookingNotes).Italic();

                            col.Item().PaddingTop(15).Text("Used Ingredients").FontSize(16).Bold();

                            if (cookingDetailViewModel.UsedIngredients.Any())
                            {
                                col.Item().Table(table =>
                                {
                                    table.ColumnsDefinition(columns =>
                                    {
                                        columns.RelativeColumn(3);
                                        columns.RelativeColumn(2);
                                        columns.RelativeColumn(2);
                                        columns.RelativeColumn(2);
                                    });

                                    table.Header(header =>
                                    {
                                        header.Cell().BorderBottom(1).Text("Ingredient").Bold();
                                        header.Cell().BorderBottom(1).Text("Quantity").Bold();
                                        header.Cell().BorderBottom(1).Text("Cost").Bold();
                                        header.Cell().BorderBottom(1).Text("Expiration Date").Bold();
                                    });

                                    foreach (var ingredient in cookingDetailViewModel.UsedIngredients)
                                    {
                                        table.Cell().Text(ingredient.IngredientName);
                                        table.Cell().Text($"{ingredient.QuantityUsed} {ingredient.Unit}");
                                        table.Cell().Text($"{ingredient.Cost:0.00} €");
                                        table.Cell().Text(ingredient.ExpirationDate.ToString("dd/MM/yyyy"));
                                    }
                                });
                            }
                            else
                            {
                                col.Item().Text("No ingredients used.").Italic();
                            }
                        });

                    page.Footer()
                        .AlignRight()
                        .Text(txt =>
                        {
                            txt.Span("Generated on: ").SemiBold();
                            txt.Span(DateTime.Now.ToString("g"));
                        });
                });
            });

            using (MemoryStream stream = new MemoryStream())
            {
                document.GeneratePdf(stream);
                return File(stream.ToArray(), "application/pdf", "CookingDetails-"+ cookingDetailViewModel.Id + "-" + cookingDetailViewModel.RecipeName + "-"+ cookingDetailViewModel.CookingDate + ".pdf");
            }
        }

        [HttpGet]
        public async Task<IActionResult> Cut(int id)
        {
            var cooking = await _context.Cookings
                .Include(c => c.ProductUnits)
                .Include(c => c.Recipe)
                .Include(c => c.UsedIngredients)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (cooking == null) return NotFound();

            cooking.CutIntoUnits();
            _context.Update(cooking);
            await _context.SaveChangesAsync();

            return RedirectToAction("Details", new { id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CutConfirm(CookingDetailViewModel model)
        {
            var cooking = await _context.Cookings.FirstOrDefaultAsync(c => c.Id == model.Id);

            if (cooking == null) return NotFound();

            if (cooking.BatchSize != model.BatchSize)
            {
                cooking.BatchSize = model.BatchSize; // Uloží novou hodnotu
                cooking.BatchSizeWasChanged = true;
            }
            cooking.IsCut = true; // Označí, že byl proveden "Cut"

            await _context.SaveChangesAsync();
            return RedirectToAction("Details", new { id = cooking.Id }); // Přesměrování na detail
        }
    }
}
