using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SoapProductionApp.Data;
using SoapProductionApp.Models.Backup;
using SoapProductionApp.Models.Warehouse;
using SoapProductionApp.Services;
using System.IO.Compression;

namespace SoapProductionApp.Controllers
{
    [Authorize(Roles = "Admin")]
    public class BackupController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IAuditService _auditService;

        public BackupController(ApplicationDbContext context, IAuditService auditService)
        {
            _context = context;
            _auditService = auditService;
        }

        [AllowAnonymous]
        public IActionResult Index()
        {
            var backupFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "backups");
            var backupList = GetBackupList(backupFolder);

            return View(backupList);
        }

        [HttpPost]
        public async Task<IActionResult> CreateBackup() 
        {
            var backupFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "backups");
            if (!Directory.Exists(backupFolder))
                Directory.CreateDirectory(backupFolder);

            var fileName = $"backup_{DateTime.Now:yyyyMMdd_HHmmss}.zip";
            var fullPath = Path.Combine(backupFolder, fileName);


            var foldersToZip = new[]
            {
                Path.Combine(Directory.GetCurrentDirectory(), "wwwroot","uploads"),
                Path.Combine(Directory.GetCurrentDirectory(), "wwwroot","Images")
            };

            var memoryStreams = new List<(string FileName, MemoryStream Stream)>();

            // Export JSON: WarehouseItems
            var warehouseItems = _context.WarehouseItems//.Include(w => w.Categories).Include(w => w.Batches) // TODO: fix cyclicity reference, maybe create DTO format
                                                   .ToList();
            var json = System.Text.Json.JsonSerializer.Serialize(warehouseItems, new System.Text.Json.JsonSerializerOptions { WriteIndented = true });
            var jsonStream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(json));
            memoryStreams.Add(("WarehouseItem.json", jsonStream));

            // Export JSON: Batches
            var batches = _context.Batches.ToList();
            json = System.Text.Json.JsonSerializer.Serialize(batches, new System.Text.Json.JsonSerializerOptions { WriteIndented = true });
            jsonStream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(json));
            memoryStreams.Add(("Batch.json", jsonStream));

            // Export JSON: Recipe
            var recipes = _context.Recipes.ToList();
            json = System.Text.Json.JsonSerializer.Serialize(recipes, new System.Text.Json.JsonSerializerOptions { WriteIndented = true });
            jsonStream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(json));
            memoryStreams.Add(("Recipe.json", jsonStream));

            // Export JSON: Cooking
            var cooking = _context.Cookings.ToList();
            json = System.Text.Json.JsonSerializer.Serialize(cooking, new System.Text.Json.JsonSerializerOptions { WriteIndented = true });
            jsonStream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(json));
            memoryStreams.Add(("Cooking.json", jsonStream));

            using (var archive = ZipFile.Open(fullPath, ZipArchiveMode.Create))
            {
                foreach (var folder in foldersToZip)
                {
                    var files = Directory.GetFiles(folder, "*", SearchOption.AllDirectories);
                    foreach (var file in files)
                    {
                        var relativePath = Path.GetRelativePath(folder, file);
                        archive.CreateEntryFromFile(file, Path.Combine(Path.GetFileName(folder), relativePath));
                        await _auditService.LogAsync("Add", "BackUp", null, null, relativePath);
                    }
                }

                // Přidání exportovaných dat (CSV/JSON)
                foreach (var (exportFileName, stream) in memoryStreams)
                {
                    stream.Position = 0;
                    var entry = archive.CreateEntry($"data/{exportFileName}");
                    using var entryStream = entry.Open();
                    stream.CopyTo(entryStream);
                }
            }
            return RedirectToAction("Index");
        }

        private List<BackupInfo> GetBackupList(string backupFolderPath)
        {
            if (!Directory.Exists(backupFolderPath))
                return new List<BackupInfo>();

            var backupFiles = Directory.GetFiles(backupFolderPath, "*.zip", SearchOption.TopDirectoryOnly);

            return backupFiles
                .Select(path => new FileInfo(path))
                .Select(file => new BackupInfo
                {
                    BackupDate = file.LastWriteTime,
                    SizeInBytes = file.Length,
                    FullPath = file.FullName,
                    FileName = file.Name
                })
                .OrderByDescending(b => b.BackupDate)
                .ToList();
        }

        public IActionResult DownloadBackup(string fileName)
        {
            if(fileName.IsNullOrEmpty())
                return RedirectToAction("Index");

            var backupFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "backups");
            var fullPath = Path.Combine(backupFolder, fileName);

            if (!System.IO.File.Exists(fullPath))
                return RedirectToAction("Index");


            var fileBytes = System.IO.File.ReadAllBytes(fullPath);
            return File(fileBytes, "application/zip", fileName);
        }
    }
}
