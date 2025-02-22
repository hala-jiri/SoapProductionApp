using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SoapProductionApp.Data;
using SoapProductionApp.Models;
using SoapProductionApp.Models.ViewModels;
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
                //var quantityBaseUnit = UnitMeasurement.ConvertToBaseUnit(batchCreateViewModel.Unit, (decimal)batchCreateViewModel.QuantityOfPackage);
                batchCreateViewModel.WarehouseItem = warehouseItem;
                batchCreateViewModel.Unit = warehouseItem.Unit;
                var batch = new Batch(batchCreateViewModel) { };

                _context.Batches.Add(batch);
                await _context.SaveChangesAsync();
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

                /*var warehouseItem = await _context.WarehouseItems
                   .FirstOrDefaultAsync(w => w.Id == batchCreateViewModel.WarehouseItemId);*/

                //var quantityBaseUnit = UnitMeasurement.ConvertToBaseUnit(batchCreateViewModel.Unit, (decimal)batchCreateViewModel.QuantityOfPackage);

                batch.Name = batchCreateViewModel.Name;
                batch.Supplier = batchCreateViewModel.Supplier;
                batch.PurchaseDate = batchCreateViewModel.PurchaseDate;
                batch.ExpirationDate = batchCreateViewModel.ExpirationDate;

                batch.TaxPercentage = batchCreateViewModel.TaxPercentage;

                batch.AvailableQuantity = batchCreateViewModel.QuantityOfPackage;
                
                batch.UnitPriceWithoutTax = batchCreateViewModel.PriceOfPackageWithoutTax / batchCreateViewModel.QuantityOfPackage;


                _context.Batches.Update(batch);
                await _context.SaveChangesAsync();

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

            int warehouseItemId = batch.WarehouseItemId;

            _context.Batches.Remove(batch);
            await _context.SaveChangesAsync();

            return RedirectToAction("Details", "Warehouse", new { id = warehouseItemId });
        }
    }
}