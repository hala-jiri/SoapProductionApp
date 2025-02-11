using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SoapProductionApp.Data;
using SoapProductionApp.Models.Warehouse;

namespace SoapProductionApp.Controllers
{
    public class BatchController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BatchController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Vytvoření nové šarže
        public IActionResult Create(int warehouseItemId)
        {
            var model = new Batch
            {
                WarehouseItemId = warehouseItemId,
                PurchaseDate = DateTime.Now
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Batch batch)
        {
            ModelState.Remove("WarehouseItem"); // Odstraníme WarehouseItem z validace TODO: vyresit

            if (ModelState.IsValid)
            {
                _context.Batches.Add(batch);
                await _context.SaveChangesAsync();
                return RedirectToAction("Details", "Warehouse", new { id = batch.WarehouseItemId });
            }
            return View(batch);
        }

        // Editace šarže
        public async Task<IActionResult> Edit(int id)
        {
            var batch = await _context.Batches.FindAsync(id);
            if (batch == null) return NotFound();
            return View(batch);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Batch batch)
        {
            if (id != batch.Id) return NotFound();

            ModelState.Remove("WarehouseItem"); // Odstraníme WarehouseItem z validace TODO: vyresit
            if (ModelState.IsValid)
            {
                _context.Batches.Update(batch);
                await _context.SaveChangesAsync();
                // Načteme WarehouseItem znovu, aby se správně zobrazila Quantity a PricePerUnit

                /*var warehouseItem = await _context.WarehouseItems
                    .Include(w => w.Batches) // Důležité: načíst i Batches
                    .FirstOrDefaultAsync(w => w.Id == batch.WarehouseItemId);*/

                return RedirectToAction("Details", "Warehouse", new { id = batch.WarehouseItemId });
            }
            return View(batch);
        }

        // Smazání šarže
        public async Task<IActionResult> Delete(int id)
        {
            var batch = await _context.Batches.FindAsync(id);
            if (batch == null) return NotFound();

            _context.Batches.Remove(batch);
            await _context.SaveChangesAsync();
            return RedirectToAction("Details", "Warehouse", new { id = batch.WarehouseItemId });
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var batch = await _context.Batches.FindAsync(id);
            if (batch == null) return NotFound();

            int warehouseItemId = batch.WarehouseItemId;

            _context.Batches.Remove(batch);
            await _context.SaveChangesAsync();

            return RedirectToAction("Details", "Warehouse", new { id = warehouseItemId });
        }
    }
}