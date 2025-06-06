using Microsoft.AspNetCore.Mvc;
using Zadanie5.DTOs;
using Zadanie5.Exceptions;
using Zadanie5.Services;

namespace Zadanie5.Controllers;

[ApiController]
[Route("api")]
public class PrescritpionsController : ControllerBase
{
    private readonly IDbService _dbService;

    public PrescritpionsController(IDbService dbService)
    {
        _dbService = dbService;
    }

    [HttpGet("patients/{patientId}")]
    public async Task<IActionResult> GetPatientAsyns(int patientId)
    {
        try
        {
            var patient = await _dbService.GetPatientAsyns(patientId);
            return Ok(patient);
        }
        catch (NotFoundException nfe)
        {
            return NotFound(nfe.Message);
        }
    }
    
    [HttpPost("prescriptions")]
    public async Task<IActionResult> AddPrescriptionAsync(AddPrescriptionDto request)
    {
        try
        {
            await _dbService.AddPrescriptionAsync(request);
            return Created();
        }
        catch (BadRequestException bre)
        {
            return BadRequest(bre.Message);
        }
        catch (NotFoundException nfe)
        {
            return NotFound(nfe.Message);
        }
    }
}