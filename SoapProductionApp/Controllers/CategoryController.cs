using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SoapProductionApp.Data;
using SoapProductionApp.Extensions;
using SoapProductionApp.Models.Warehouse;
using SoapProductionApp.Services;

namespace SoapProductionApp.Controllers
{
    [Authorize(Roles = "Admin")]
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IAuditService _auditService;

        public CategoryController(ApplicationDbContext context, IAuditService auditService)
        {
            _context = context;
            _auditService = auditService;
        }

        public IActionResult Index()
        {
            var categories = _context.Categories.ToList();
            return View(categories);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Category category)
        {
            if (ModelState.IsValid)
            {
                _context.Categories.Add(category);
                _context.SaveChanges();

                await _auditService.LogAsync("Add", "Category", category.Id, null, category.ToSafeJson());
                return RedirectToAction("Index");
            }
            return View(category);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,ColorBackground,ColorText")] Category category)
        {
            if (id != category.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingCategory = await _context.Categories.FindAsync(id);
                    if (existingCategory == null) return NotFound();

                    var existingCategoryJson = existingCategory.ToSafeJson();

                    existingCategory.Name = category.Name;
                    existingCategory.ColorBackground = category.ColorBackground;
                    existingCategory.ColorText = category.ColorText;

                    await _context.SaveChangesAsync();

                    var afterChangeCategoryJson = category.ToSafeJson();
                    await _auditService.LogAsync("Update", "Category", id, existingCategoryJson, afterChangeCategoryJson);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Categories.Any(e => e.Id == category.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        public IActionResult Delete(int id)
        {
            var category = _context.Categories.Find(id);
            if (category == null) return NotFound();
            return View(category);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var category = _context.Categories.Find(id);
            if (category != null)
            {
                var beforeJson = category.ToSafeJson();

                _context.Categories.Remove(category);
                _context.SaveChanges();

                await _auditService.LogAsync("Remove", "Category", id, category.ToSafeJson(), null);
            }
            return RedirectToAction("Index");
        }
    }
}
