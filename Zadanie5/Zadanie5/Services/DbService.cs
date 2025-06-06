using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Zadanie5.DAL;
using Zadanie5.DTOs;
using Zadanie5.Exceptions;
using Zadanie5.Models;

namespace Zadanie5.Services;

public class DbService : IDbService
{
    private readonly DatabaseContext _dbContext;

    public DbService(DatabaseContext dbContext)
    {
        _dbContext = dbContext;
    }
     
    public async Task AddPrescriptionAsync(AddPrescriptionDto requestDto)
    {
        var patient = await _dbContext.Patients.FirstOrDefaultAsync(p => p.IdPatient == requestDto.patient.IdPatient);

        if (patient is null)
        {
            patient = new Patient
            {
                FirstName = requestDto.patient.FirstName,
                LastName = requestDto.patient.LastName,
                BirthDate = requestDto.patient.BirthDate,
            };
            
            await _dbContext.Patients.AddAsync(patient);
        }

        foreach (MedicamentDto medicamentDto in requestDto.medicaments)
        {
            if (!await _dbContext.Medicaments.AnyAsync(m => m.IdMedicament == medicamentDto.IdMedicament))
            {
                throw new NotFoundException($"Medicament with given ID - {medicamentDto.IdMedicament} doesn't exist");
            }
        }

        if (requestDto.medicaments.Count > 10)
        {
            throw new BadRequestException("A prescription can cover up to 10 medicaments");
        }

        if (requestDto.Date > requestDto.DueDate)
        {
            throw new BadRequestException("A DueDate should be equal or greater than Date");
        }
      
        var prescription = new Prescription
        {
            Date = requestDto.Date,
            DueDate = requestDto.DueDate,
            Patient = patient,
            Doctor = await _dbContext.Doctors.FirstAsync()
        };
        await _dbContext.Prescriptions.AddAsync(prescription);

        foreach (MedicamentDto medicamentDto in requestDto.medicaments)
        {
            var prescriptionMedicament = new PrescriptionMedicament
            {
                Dose = medicamentDto.Dose,
                Details = medicamentDto.Description,
                Medicament = await _dbContext.Medicaments.FirstAsync(m => m.IdMedicament == medicamentDto.IdMedicament),
                Prescription = prescription
            };
            
            await _dbContext.PrescriptionMedicaments.AddAsync(prescriptionMedicament);
        }
        
        await _dbContext.SaveChangesAsync();
    }

    public async Task<GetPatientInfoDto> GetPatientAsyns(int patientId)
    {
        var patient = await _dbContext.Patients.FirstOrDefaultAsync(p => p.IdPatient == patientId);

        if (patient is null)
        {
            throw new NotFoundException($"Patient with given ID - {patientId} doesn't exist");
        }
        
        var prescriptions = await _dbContext.Prescriptions.Where(p => p.IdPatient == patientId).ToListAsync();
        
        List<PrescriptionDto> prescriptionsDto = new List<PrescriptionDto>();
        
        foreach (Prescription prescription in prescriptions)
        {
            List<PrescriptionMedicamentDto> prescriptionMedicamentsDtos = new List<PrescriptionMedicamentDto>();
            
            var prescriptionMedicament = await _dbContext.PrescriptionMedicaments
                .Join(_dbContext.Medicaments, pm => pm.IdMedicament, m => m.IdMedicament, (pm, m) => new {pm.IdPrecription, m.IdMedicament, m.Name, pm.Dose, pm.Details})
                .Where(pm => pm.IdPrecription == prescription.IdPrescription).ToListAsync();


            foreach (var pm in prescriptionMedicament)
            {
                prescriptionMedicamentsDtos.Add(new PrescriptionMedicamentDto
                {
                    IdMedicament = pm.IdMedicament,
                    Name = pm.Name,
                    Dose = pm.Dose,
                    Description = pm.Details
                });
            }
            
            var doctor = await _dbContext.Doctors.FirstAsync(d => d.IdDoctor == prescription.IdDoctor);
            
            prescriptionsDto.Add(new PrescriptionDto
            {
                IdPrescription = prescription.IdPrescription,
                Date = prescription.Date,
                DueDate = prescription.DueDate,
                Medicaments = prescriptionMedicamentsDtos,
                Doctor = new DoctorDto
                {
                    IdDoctor = doctor.IdDoctor,
                    FirstName = doctor.FirstName,
                    LastName = doctor.LastName,
                    Email = doctor.Email
                }
            });
        }
        
        var sortedPrescriptionsDto = prescriptionsDto.OrderByDescending(p => p.DueDate).ToList();

        var patientDto = new GetPatientInfoDto
        {
            IdPatient = patient.IdPatient,
            FirstName = patient.FirstName,
            LastName = patient.LastName,
            BirthDate = patient.BirthDate,
            Prescriptions = sortedPrescriptionsDto
        };
        
        return patientDto;
    }
}