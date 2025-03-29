using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SoapProductionApp.Data;
using SoapProductionApp.Models.Backup;
using System.IO.Compression;

namespace SoapProductionApp.Controllers
{
    public class BackupController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BackupController(ApplicationDbContext context)
        {
            _context = context;
        }

        [AllowAnonymous]
        public IActionResult Index()
        {
            var backupFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "backups");
            var backupList = GetBackupList(backupFolder);

            return View(backupList);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult CreateBackup() 
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

            using (var archive = ZipFile.Open(fullPath, ZipArchiveMode.Create))
            {
                foreach (var folder in foldersToZip)
                {
                    var files = Directory.GetFiles(folder, "*", SearchOption.AllDirectories);
                    foreach (var file in files)
                    {
                        var relativePath = Path.GetRelativePath(folder, file);
                        archive.CreateEntryFromFile(file, Path.Combine(Path.GetFileName(folder), relativePath));
                    }
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
                    FullPath = file.FullName
                })
                .OrderByDescending(b => b.BackupDate)
                .ToList();
        }
    }
}
