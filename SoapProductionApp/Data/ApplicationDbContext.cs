using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SoapProductionApp.Models.Warehouse;

namespace SoapProductionApp.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public DbSet<WarehouseItem> WarehouseItems { get; set; }
        public DbSet<WarehouseItemBatch> WarehouseItemBatches { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
    }
}
