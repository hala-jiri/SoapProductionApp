using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SoapProductionApp.Data;
using SoapProductionApp.Extensions;
using SoapProductionApp.Models.Warehouse;
using SoapProductionApp.Models.Warehouse.ViewModels;
using SoapProductionApp.Services;

namespace SoapProductionApp.Controllers
{
    public class BatchController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IAuditService _auditService;

        public BatchController(ApplicationDbContext context, IAuditService auditService)
        {
            _context = context;
            _auditService = auditService;
        }

        // GET: Batch/Create
        public async Task<IActionResult> Create(int warehouseItemId)
        {
            var warehouseItem = await _context.WarehouseItems.FirstOrDefaultAsync(w => w.Id == warehouseItemId);

            if (warehouseItem == null)
            {
                return NotFound();
            }

            ViewBag.Unit = warehouseItem.Unit;
            ViewBag.WarehouseItemName = warehouseItem.Name;
            ViewBag.WarehouseItemId = warehouseItem.Id;
            ViewBag.Tax = warehouseItem.TaxPercentage;

            var batchCreateViewModel = new BatchCreateViewModel() { };
            return View(batchCreateViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BatchCreateViewModel batchCreateViewModel)
        {
            ModelState.Remove("WarehouseItem");
            if (ModelState.IsValid)
            {
                var warehouseItem = await _context.WarehouseItems
                   .FirstOrDefaultAsync(w => w.Id == batchCreateViewModel.WarehouseItemId);
                if (warehouseItem == null)
                {
                    return View(batchCreateViewModel);
                }

                batchCreateViewModel.WarehouseItem = warehouseItem;
                batchCreateViewModel.Unit = warehouseItem.Unit;
                batchCreateViewModel.TaxPercentage = warehouseItem.TaxPercentage;
                var batch = new Batch(batchCreateViewModel, warehouseItem) { };
                var batchInJson = batch.ToSafeJson();

                _context.Batches.Add(batch);
                await _context.SaveChangesAsync();
                await _auditService.LogAsync("Add", "Batch", batch.Id, null, batchInJson);

                return RedirectToAction("Details", "Warehouse", new { id = batch.WarehouseItemId });
            }
            return View(batchCreateViewModel);
        }

        // GET: Batch/Edit
        public async Task<IActionResult> Edit(int id)
        {
            var batch = await _context.Batches
                .Include(b => b.WarehouseItem)
                .FirstOrDefaultAsync(b => b.Id == id);
            if (batch == null) return NotFound();

            var batchCreateViewModel = new BatchCreateViewModel(batch);

            ViewBag.Unit = batch.Unit;
            return View(batchCreateViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, BatchCreateViewModel batchCreateViewModel)
        {
            if (id != batchCreateViewModel.Id) return NotFound();
            ModelState.Remove("WarehouseItem");
            if (ModelState.IsValid)
            {
                var batch = await _context.Batches.FirstOrDefaultAsync(i => i.Id == id);
                if (batch == null) return NotFound();
                var batchBeforeJson = batch.ToSafeJson();

                batch.Name = batchCreateViewModel.Name;
                batch.Supplier = batchCreateViewModel.Supplier;
                batch.PurchaseDate = batchCreateViewModel.PurchaseDate;
                batch.ExpirationDate = batchCreateViewModel.ExpirationDate;
                batch.TaxPercentage = batchCreateViewModel.TaxPercentage;
                batch.AvailableQuantity = batchCreateViewModel.QuantityOfPackage;
                batch.UnitPriceWithoutTax = batchCreateViewModel.PriceOfPackageWithoutTax / batchCreateViewModel.QuantityOfPackage;

                _context.Batches.Update(batch);
                await _context.SaveChangesAsync();
                
                var batchAfterJson = batch.ToSafeJson();
                await _auditService.LogAsync("Update", "Batch", batch.Id, batchBeforeJson, batchAfterJson);

                return RedirectToAction("Details", "Warehouse", new { id = batchCreateViewModel.WarehouseItemId });
            }
            return View(batchCreateViewModel);
        }

        // GET: Batch/Delete
        public async Task<IActionResult> Delete(int id)
        {
            var batch = await _context.Batches
                .Include(b => b.WarehouseItem)
                .FirstOrDefaultAsync(b => b.Id == id);
            if (batch == null) return NotFound();

            var batchViewModel = new BatchViewModel(batch);

            return View(batchViewModel);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var batch = await _context.Batches
                .Include(b => b.WarehouseItem)
                .FirstOrDefaultAsync(b => b.Id == id);
            if (batch == null) return NotFound();

            var batchBeforeJson = batch.ToSafeJson();
            int warehouseItemId = batch.WarehouseItemId;

            _context.Batches.Remove(batch);
            await _context.SaveChangesAsync();

            await _auditService.LogAsync("Remove", "Batch", batch.Id, batchBeforeJson, null);
            return RedirectToAction("Details", "Warehouse", new { id = warehouseItemId });
        }
    }
}