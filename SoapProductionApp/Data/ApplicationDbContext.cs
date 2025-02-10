using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SoapProductionApp.Models.Warehouse;

namespace SoapProductionApp.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<WarehouseItem> WarehouseItems { get; set; }
        public DbSet<WarehouseItemBatch> WarehouseItemBatches { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Unit> Units { get; set; }
        public DbSet<Batch> Batches { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Konfigurace M:N vztahu mezi WarehouseItem a Category
            modelBuilder.Entity<WarehouseItem>()
                .HasMany(w => w.Categories);
                //.WithMany(c => c.WarehouseItems);
        }
    }
}
