using Microsoft.EntityFrameworkCore;
using APBD_CW6.Models;

namespace APBD_CW6.Data
{
    public class ApplicationDbContext : DbContext
    {
        // Konstruktor z opcjami DbContext
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        // Zdefiniowanie zestawów danych dla modeli
        public DbSet<Patient> Patients { get; set; }
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<Medicament> Medicaments { get; set; }
        public DbSet<Prescription> Prescriptions { get; set; }
        public DbSet<PrescriptionMedicament> PrescriptionMedicaments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Konfiguracja relacji wiele do wielu między Prescription a Medicament
            modelBuilder.Entity<PrescriptionMedicament>()
                .HasKey(pm => new { pm.PrescriptionId, pm.MedicamentId });

            modelBuilder.Entity<PrescriptionMedicament>()
                .HasOne(pm => pm.Prescription)
                .WithMany(p => p.PrescriptionMedicaments)
                .HasForeignKey(pm => pm.PrescriptionId);

            modelBuilder.Entity<PrescriptionMedicament>()
                .HasOne(pm => pm.Medicament)
                .WithMany(m => m.PrescriptionMedicaments)
                .HasForeignKey(pm => pm.MedicamentId);
        }
    }
}