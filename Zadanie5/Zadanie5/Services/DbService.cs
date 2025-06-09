using Microsoft.EntityFrameworkCore;
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
        using var transaction = await _dbContext.Database.BeginTransactionAsync();

        try
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
            await transaction.CommitAsync();
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<PatientInfoDto> GetPatientAsyns(int patientId)
    {
        
        var patientInfoDto = await _dbContext.Patients.Select(p => new PatientInfoDto
        {
            IdPatient = p.IdPatient,
            FirstName = p.FirstName,
            LastName = p.LastName,
            BirthDate = p.BirthDate,
            Prescriptions = p.Prescriptions.Select(pre => new PrescriptionDto
            {
                IdPrescription = pre.IdPrescription,
                Date = pre.Date,
                DueDate = pre.DueDate,
                Medicaments = pre.PrescriptionsMedicaments.Select(preM => new PrescriptionMedicamentDto
                {
                    IdMedicament = preM.Medicament.IdMedicament,
                    Name = preM.Medicament.Name,
                    Dose = preM.Dose,
                    Description = preM.Details
                }).ToList(),
                Doctor = new DoctorDto()
                {
                    IdDoctor = pre.Doctor.IdDoctor,
                    FirstName = pre.Doctor.FirstName,
                    LastName = pre.Doctor.LastName,
                    Email = pre.Doctor.Email
                }
            }).ToList()
        }).FirstOrDefaultAsync(p => p.IdPatient == patientId);

        if (patientInfoDto is null)
        {
            throw new NotFoundException($"Patient with given ID - {patientId} doesn't exist");
        }
        
        return patientInfoDto;
    }
}