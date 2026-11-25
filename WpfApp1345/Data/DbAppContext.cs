
using Microsoft.EntityFrameworkCore;
using WpfApp1345.Models;

namespace WpfApp1345.Data
{
    public class DbAppContext : DbContext
    {
        public DbSet<Phone> Phones { get; set; } = null!;

        public DbSet<Company> Companies { get; set; } = null!;

        protected override void OnConfiguring(
            DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseNpgsql(
                    "Host=localhost;Port=5432;Database=PhoneStore;Username=postgres;Password=123");
            }
        }

        protected override void OnModelCreating(
            ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Связь: одна компания — много телефонов.
            modelBuilder.Entity<Phone>()
                .HasOne(p => p.CompanyEntity)
                .WithMany(c => c.PhoneEntities)
                .HasForeignKey(p => p.CompanyId)
                .OnDelete(DeleteBehavior.Restrict);

            // Описание телефона.
            modelBuilder.Entity<Phone>()
                .Property(p => p.Definition)
                .HasDefaultValue("");

            // Изображение телефона.
            // Имя столбца в PostgreSQL — Image.
            modelBuilder.Entity<Phone>()
                .Property(p => p.ImageSource)
                .HasColumnName("Image")
                .HasDefaultValue("No image");
        }
    }
}
