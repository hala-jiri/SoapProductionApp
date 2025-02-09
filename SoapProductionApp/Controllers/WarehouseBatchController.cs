using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SoapProductionApp.Data;
using SoapProductionApp.Models;
using SoapProductionApp.Models.Warehouse;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SoapProductionApp.Controllers
{
    [Authorize]
    public class WarehouseBatchController : Controller
    {
        private readonly ApplicationDbContext _context;

        public WarehouseBatchController(ApplicationDbContext context)
        {
            _context = context;
        }

        // 🟢 SEZNAM VŠECH ŠARŽÍ
        public async Task<IActionResult> Index()
        {
            var batches = await _context.WarehouseItemBatches
                .Include(b => b.WarehouseItem)
                .OrderByDescending(b => b.PurchaseDate)
                .ToListAsync();

            return View(batches);
        }

        // Detail konkrétního WarehouseItemBatch
        public async Task<IActionResult> Details(int id)
        {
            var batch = await _context.WarehouseItemBatches
                .Include(b => b.WarehouseItem)
                .ThenInclude(w => w.Unit)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (batch == null)
            {
                return NotFound();
            }

            return View(batch);
        }

        // 🟢 VYTVOŘENÍ ŠARŽE
        public async Task<IActionResult> Create(int warehouseItemId)
        {
            var item = await _context.WarehouseItems.FindAsync(warehouseItemId);
            if (item == null) return NotFound();

            ViewBag.WarehouseItem = item;
            return View(new WarehouseItemBatch { WarehouseItemId = warehouseItemId });
        }

        [HttpPost]
        public async Task<IActionResult> Create(WarehouseItemBatch batch)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.WarehouseItem = await _context.WarehouseItems.FindAsync(batch.WarehouseItemId);
                return View(batch);
            }

            // VALIDACE
            if (batch.Quantity <= 0)
            {
                ModelState.AddModelError("Quantity", "Množství musí být větší než 0.");
                ViewBag.WarehouseItem = await _context.WarehouseItems.FindAsync(batch.WarehouseItemId);
                return View(batch);
            }

            if (batch.PricePerUnit < 0)
            {
                ModelState.AddModelError("PricePerUnit", "Cena nesmí být záporná.");
                ViewBag.WarehouseItem = await _context.WarehouseItems.FindAsync(batch.WarehouseItemId);
                return View(batch);
            }

            // Přidání nové šarže
            _context.WarehouseItemBatches.Add(batch);

            // Aktualizace množství v hlavní tabulce položek
            //var item = await _context.WarehouseItems.FindAsync(batch.WarehouseItemId);
            //if (item != null)
            //{
            //    item.Quantity += batch.Quantity;
            //}

            await _context.SaveChangesAsync();
            return RedirectToAction("Details", "Warehouse", new { id = batch.WarehouseItemId });
        }

        // 🟠 EDITACE ŠARŽE
        public async Task<IActionResult> Edit(int id)
        {
            var batch = await _context.WarehouseItemBatches.FindAsync(id);
            if (batch == null) return NotFound();

            return View(batch);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(WarehouseItemBatch batch)
        {
            if (!ModelState.IsValid) return View(batch);

            _context.WarehouseItemBatches.Update(batch);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        // 🔴 SMAZÁNÍ ŠARŽE
        public async Task<IActionResult> Delete(int id)
        {
            var batch = await _context.WarehouseItemBatches
                .Include(b => b.WarehouseItem)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (batch == null) return NotFound();
            return View(batch);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var batch = await _context.WarehouseItemBatches.FindAsync(id);
            if (batch == null) return NotFound();

            _context.WarehouseItemBatches.Remove(batch);

            // Snížení množství položky
            var item = await _context.WarehouseItems.FindAsync(batch.WarehouseItemId);
            //if (item != null)
            //{
            //    item.Quantity -= batch.Quantity;
            //}

            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        // SPOTŘEBA MATERIÁLU (FIFO LOGIKA)
        public async Task<bool> ConsumeMaterial(int itemId, decimal quantity)
        {
            var batches = await _context.WarehouseItemBatches
                .Where(b => b.WarehouseItemId == itemId && b.Quantity > 0)
                .OrderBy(b => b.ExpirationDate)
                .ToListAsync();

            decimal originalQuantity = quantity;

            foreach (var batch in batches)
            {
                if (quantity <= 0) break;

                if (batch.Quantity >= quantity)
                {
                    batch.Quantity -= quantity;
                    quantity = 0;
                }
                else
                {
                    quantity -= batch.Quantity;
                    batch.Quantity = 0;
                }
            }

            await _context.SaveChangesAsync();
            return originalQuantity > quantity; // Vrací true, pokud se podařilo spotřebovat vše.
        }
    }
}