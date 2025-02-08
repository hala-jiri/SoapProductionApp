using Microsoft.AspNetCore.Mvc;
using SoapProductionApp.Data;
using SoapProductionApp.Models.Warehouse;

namespace SoapProductionApp.Controllers
{
    public class UnitController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UnitController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var units = _context.Units.ToList();
            return View(units);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Unit unit)
        {
            if (ModelState.IsValid)
            {
                _context.Units.Add(unit);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(unit);
        }

        public IActionResult Edit(int id)
        {
            var unit = _context.Units.Find(id);
            if (unit == null) return NotFound();
            return View(unit);
        }

        [HttpPost]
        public IActionResult Edit(Unit unit)
        {
            if (ModelState.IsValid)
            {
                _context.Units.Update(unit);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(unit);
        }

        public IActionResult Delete(int id)
        {
            var unit = _context.Units.Find(id);
            if (unit == null) return NotFound();
            return View(unit);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            var unit = _context.Units.Find(id);
            if (unit != null)
            {
                _context.Units.Remove(unit);
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }
    }
}
