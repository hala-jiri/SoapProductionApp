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
            if (ModelState.IsValid)
            {
                // Přidání nové zásoby
                _context.WarehouseItemBatches.Add(batch);

                // Aktualizace celkového množství skladové položky
                var item = await _context.WarehouseItems.FindAsync(batch.WarehouseItemId);
                if (item != null)
                {
                    item.Quantity += batch.Quantity;
                    _context.WarehouseItems.Update(item);
                }

                await _context.SaveChangesAsync();
                return RedirectToAction("Details", "Warehouse", new { id = batch.WarehouseItemId });
            }
            return View(batch);
        }

        public async Task<bool> ConsumeMaterial(int itemId, decimal quantity)
        {
            var batches = await _context.WarehouseItemBatches
                .Where(b => b.WarehouseItemId == itemId && b.Quantity > 0)
                .OrderBy(b => b.ExpirationDate)
                .ToListAsync();

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
            return quantity == 0; // Vrátí true, pokud bylo vše pokryto
        }
    }
}