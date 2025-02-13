using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SoapProductionApp.Data;
using SoapProductionApp.Models;
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

        // GET: Batch/Create
        public IActionResult Create(int warehouseItemId)
        {
            var warehouseItem = _context.WarehouseItems.Find(warehouseItemId);
            if (warehouseItem == null) return NotFound();

            var batch = new Batch
            {
                WarehouseItemId = warehouseItemId,
                WarehouseItem = warehouseItem
            };

            return View(batch);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Batch batch)
        {
            if (ModelState.IsValid)
            {
                // Přepočet na základní jednotky před uložením
                batch.QuantityInBaseUnit = UnitMeasurement.ConvertToBaseUnit(batch.Unit, (decimal)batch.QuantityOfPackage);
                batch.PricePerBaseUnit = batch.PriceOfPackage / batch.QuantityInBaseUnit;

                batch.AvailableQuantity = batch.QuantityInBaseUnit;

                _context.Batches.Add(batch);
                await _context.SaveChangesAsync();
                return RedirectToAction("Details", "Warehouse", new { id = batch.WarehouseItemId });
            }
            return View(batch);
        }

        // GET: Batch/Edit
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

            if (ModelState.IsValid)
            {
                // Přepočet na základní jednotky před uložením
                batch.QuantityInBaseUnit = UnitMeasurement.ConvertToBaseUnit(batch.Unit, (decimal)batch.QuantityOfPackage);
                batch.PricePerBaseUnit = batch.PriceOfPackage / batch.QuantityInBaseUnit;

                _context.Batches.Update(batch);
                await _context.SaveChangesAsync();

                return RedirectToAction("Details", "Warehouse", new { id = batch.WarehouseItemId });
            }
            return View(batch);
        }

        // GET: Batch/Delete
        public async Task<IActionResult> Delete(int id)
        {
            var batch = await _context.Batches.FindAsync(id);
            if (batch == null) return NotFound();

            return View(batch);
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