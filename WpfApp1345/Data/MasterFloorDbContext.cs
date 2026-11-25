
using Microsoft.EntityFrameworkCore;
using WpfApp1345.Models.MasterFloor;

namespace WpfApp1345.Data
{
    public class MasterFloorDbContext : DbContext
    {
        public DbSet<PartnerType> PartnerTypes { get; set; } = null!;

        public DbSet<Partner> Partners { get; set; } = null!;

        public DbSet<ProductType> ProductTypes { get; set; } = null!;

        public DbSet<Product> Products { get; set; } = null!;

        public DbSet<MaterialType> MaterialTypes { get; set; } = null!;

        public DbSet<PartnerSale> PartnerSales { get; set; } = null!;

        protected override void OnConfiguring(
            DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseNpgsql(
                    "Host=localhost;Port=5432;Database=MasterFloor;Username=postgres;Password=123");
            }
        }

        protected override void OnModelCreating(
            ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Типы партнеров.
            modelBuilder.Entity<PartnerType>(entity =>
            {
                entity.ToTable("PartnerTypes");

                entity.HasKey(p => p.Id);

                entity.Property(p => p.Name)
                    .IsRequired();

                entity.HasIndex(p => p.Name)
                    .IsUnique();
            });

            // Партнеры.
            modelBuilder.Entity<Partner>(entity =>
            {
                entity.ToTable("Partners");

                entity.HasKey(p => p.Id);

                entity.Property(p => p.Name)
                    .IsRequired();

                entity.Property(p => p.Director)
                    .IsRequired();

                entity.Property(p => p.Email)
                    .IsRequired();

                entity.Property(p => p.Phone)
                    .IsRequired();

                entity.Property(p => p.LegalAddress)
                    .IsRequired();

                entity.Property(p => p.INN)
                    .IsRequired();

                entity.HasIndex(p => p.INN)
                    .IsUnique();

                entity.HasOne(p => p.PartnerType)
                    .WithMany(t => t.Partners)
                    .HasForeignKey(p => p.PartnerTypeId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Типы продукции.
            modelBuilder.Entity<ProductType>(entity =>
            {
                entity.ToTable("ProductTypes");

                entity.HasKey(p => p.Id);

                entity.Property(p => p.Name)
                    .IsRequired();

                entity.Property(p => p.Coefficient)
                    .HasPrecision(10, 4);

                entity.HasIndex(p => p.Name)
                    .IsUnique();
            });

            // Продукция.
            modelBuilder.Entity<Product>(entity =>
            {
                entity.ToTable("Products");

                entity.HasKey(p => p.Id);

                entity.Property(p => p.Name)
                    .IsRequired();

                entity.Property(p => p.MinPartnerPrice)
                    .HasPrecision(18, 2);

                entity.HasIndex(p => p.Article)
                    .IsUnique();

                entity.HasOne(p => p.ProductType)
                    .WithMany(t => t.Products)
                    .HasForeignKey(p => p.ProductTypeId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Типы материалов.
            modelBuilder.Entity<MaterialType>(entity =>
            {
                entity.ToTable("MaterialTypes");

                entity.HasKey(m => m.Id);

                entity.Property(m => m.Name)
                    .IsRequired();

                entity.Property(m => m.ScrapRate)
                    .HasPrecision(10, 5);

                entity.HasIndex(m => m.Name)
                    .IsUnique();
            });

            // История продаж партнеров.
            modelBuilder.Entity<PartnerSale>(entity =>
            {
                entity.ToTable("PartnerSales");

                entity.HasKey(s => s.Id);

                entity.Property(s => s.SaleDate)
                    .HasColumnType("date");

                entity.HasOne(s => s.Partner)
                    .WithMany(p => p.Sales)
                    .HasForeignKey(s => s.PartnerId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(s => s.Product)
                    .WithMany(p => p.PartnerSales)
                    .HasForeignKey(s => s.ProductId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(s => new
                {
                    s.PartnerId,
                    s.SaleDate
                });
            });
        }
    }
}
