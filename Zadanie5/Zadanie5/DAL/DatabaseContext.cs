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
        modelBuilder.Entity<Doctor>(d =>
        {
            d.ToTable("Doctor");
            
            d.HasKey(e => e.IdDoctor);
            d.Property(e => e.FirstName).HasMaxLength(100);
            d.Property(e => e.LastName).HasMaxLength(100);
            d.Property(e => e.Email).HasMaxLength(100);
        });
        
        modelBuilder.Entity<Doctor>().HasData(new List<Doctor>
        {
            new Doctor() { IdDoctor = 1, FirstName = "John", LastName = "Meyer", Email = "john.meyer@gmail.com" }
        });
        
        modelBuilder.Entity<Patient>(p =>
        {
            p.ToTable("Patient");
            
            p.HasKey(e => e.IdPatient);
            p.Property(e => e.FirstName).HasMaxLength(100);
            p.Property(e => e.LastName).HasMaxLength(100);
            p.Property(e => e.BirthDate).HasMaxLength(100);
        });
        
        modelBuilder.Entity<Patient>().HasData(new List<Patient>
        {
            new Patient() { IdPatient = 1, FirstName = "Raiden", LastName = "Gaines", BirthDate = DateTime.Parse("01/01/1990") }
        });
        
        modelBuilder.Entity<Medicament>(p =>
        {
            p.ToTable("Medicament");
            
            p.HasKey(e => e.IdMedicament);
            p.Property(e => e.Name).HasMaxLength(100);
            p.Property(e => e.Description).HasMaxLength(100);
            p.Property(e => e.Type).HasMaxLength(100);
        });
        
        modelBuilder.Entity<Medicament>().HasData(new List<Medicament>
        {
            new Medicament() { IdMedicament = 1, Name = "TestMedicament1", Description = "TestDescription1", Type = "TestType1"},
            new Medicament() { IdMedicament = 2, Name = "TestMedicament2", Description = "TestDescription2", Type = "TestType2"},
            new Medicament() { IdMedicament = 3, Name = "TestMedicament3", Description = "TestDescription3", Type = "TestType3"},
        });


    }
}