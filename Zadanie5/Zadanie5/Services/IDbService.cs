using Zadanie5.DTOs;

namespace Zadanie5.Services;

public interface IDbService
{
    Task AddPrescriptionAsync(AddPrescriptionDto requestDto);
    Task<PatientInfoDto> GetPatientAsyns(int patientId);
}