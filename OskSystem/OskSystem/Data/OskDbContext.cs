using System.Configuration;
using Microsoft.EntityFrameworkCore;
using OskSystem.Models;

namespace OskSystem.Data
{
    public class OskDbContext : DbContext
    {
        public DbSet<Kursant> Kursanci { get; set; } = null!;
        public DbSet<Instruktor> Instruktorzy { get; set; } = null!;
        public DbSet<Pojazd> Pojazdy { get; set; } = null!;
        public DbSet<RezerwacjaJazdy> RezerwacjeJazd { get; set; } = null!;
        public DbSet<Administrator> Administratorzy { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string? connStr = ConfigurationManager.ConnectionStrings["OskBaza"]?.ConnectionString;
            optionsBuilder.UseSqlServer(connStr ?? "Server=(localdb)\\mssqllocaldb;Database=OskBazaDanych;Trusted_Connection=True;TrustServerCertificate=True;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Dwa kursanty nie mogą mieć tego samego numeru PESEL.
            modelBuilder.Entity<Kursant>()
                .HasIndex(k => k.Pesel)
                .IsUnique();

            // Każdy login musi być unikalny w swojej grupie użytkowników.
            modelBuilder.Entity<Kursant>()
                .HasIndex(k => k.Login)
                .IsUnique();

            modelBuilder.Entity<Instruktor>()
                .HasIndex(i => i.Login)
                .IsUnique();

            modelBuilder.Entity<Administrator>()
                .HasIndex(a => a.Login)
                .IsUnique();

            // Zabezpieczenie przed przypadkowym usunięciem — nie można usunąć kursanta,
            // instruktora ani pojazdu jeśli mają przypisane jazdy w systemie.
            modelBuilder.Entity<RezerwacjaJazdy>()
                .HasOne(r => r.Kursant)
                .WithMany()
                .HasForeignKey(r => r.KursantId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<RezerwacjaJazdy>()
                .HasOne(r => r.Instruktor)
                .WithMany()
                .HasForeignKey(r => r.InstruktorId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<RezerwacjaJazdy>()
                .HasOne(r => r.Pojazd)
                .WithMany()
                .HasForeignKey(r => r.PojazdId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
