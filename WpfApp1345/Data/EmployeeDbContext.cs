
using Microsoft.EntityFrameworkCore;
using WpfApp1345.Models;

namespace WpfApp1345.Data
{
    public class EmployeeDbContext : DbContext
    {
        public DbSet<Employee> Employee { get; set; } = null!;

        protected override void OnConfiguring(
            DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseNpgsql(
                    "Host=localhost;Port=5432;Database=EmpDB;Username=postgres;Password=123");
            }
        }

        protected override void OnModelCreating(
            ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Название таблицы строго Employee.
            modelBuilder.Entity<Employee>()
                .ToTable("Employee");

            // Первичный ключ.
            modelBuilder.Entity<Employee>()
                .HasKey(e => e.Id);
        }
    }
}
