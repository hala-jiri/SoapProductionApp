using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SoapProductionApp.Data;

namespace SoapProductionApp.Controllers
{
    public class AuditLogController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AuditLogController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var logs = await _context.AuditLogs
                .OrderByDescending(l => l.Timestamp)
                .Take(200)
                .ToListAsync();

            return View(logs);
        }
    }
}
