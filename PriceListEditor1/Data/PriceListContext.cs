using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using PriceListEditor1.Models;

namespace PriceListEditor1.Data
{
    public class PriceListContext : DbContext
    {
        public PriceListContext(DbContextOptions<PriceListContext> options) : base(options) { }

        public DbSet<PriceList> PriceLists { get; set; }
        public DbSet<PriceListColumn> PriceListColumns { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Column> Columns { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>()
                .Property(p => p.DynamicColumns)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, new JsonSerializerOptions { WriteIndented = false }),
                    v => JsonSerializer.Deserialize<Dictionary<string, string>>(v, new JsonSerializerOptions { WriteIndented = false })
                );

            modelBuilder.Entity<PriceList>()
                .HasMany(p => p.Columns)
                .WithOne(c => c.PriceList)
                .HasForeignKey(c => c.PriceListId)
                .OnDelete(DeleteBehavior.Cascade);

            base.OnModelCreating(modelBuilder);
        }
    }
}
