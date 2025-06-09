using Microsoft.EntityFrameworkCore;
using Zadanie5.Models;

namespace Zadanie5.DAL;

public class DatabaseContext: DbContext
{
    
    public DbSet<Doctor> Doctors { get; set; }
    public DbSet<Medicament> Medicaments { get; set; }
    public DbSet<Patient> Patients { get; set; }
    public DbSet<Prescription> Prescriptions { get; set; }
    public DbSet<PrescriptionMedicament> PrescriptionMedicaments { get; set; }
    
    protected DatabaseContext()
    {
    }

    public DatabaseContext(DbContextOptions options) : base(options)
    {
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Doctor>().HasData(new List<Doctor>
        {
            new Doctor() { IdDoctor = 1, FirstName = "John", LastName = "Meyer", Email = "john.meyer@gmail.com" }
        });
        
        modelBuilder.Entity<Patient>().HasData(new List<Patient>
        {
            new Patient() { IdPatient = 1, FirstName = "Raiden", LastName = "Gaines", BirthDate = DateTime.Parse("1990-01-01") }
        });
        
        modelBuilder.Entity<Medicament>().HasData(new List<Medicament>
        {
            new Medicament() { IdMedicament = 1, Name = "TestMedicament1", Description = "", Type = ""},
            new Medicament() { IdMedicament = 2, Name = "TestMedicament2", Description = "", Type = ""},
            new Medicament() { IdMedicament = 3, Name = "TestMedicament3", Description = "", Type = ""},
        });
    }
}