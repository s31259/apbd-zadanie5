using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Zadanie5.Models;

[PrimaryKey(nameof(IdPrescription))]
[Table("Prescription")]
public class Prescription
{   
    [Required]
    [Key]
    public int IdPrescription { get; set; }   

    [Required]
    public DateTime Date { get; set; }
    
    [Required]
    public DateTime DueDate { get; set; }
    
    [Required]
    [ForeignKey(nameof(Patient))]
    public int IdPatient { get; set; }
    
    [Required]
    [ForeignKey(nameof(Doctor))]
    public int IdDoctor { get; set; }
    
    public Patient Patient { get; set; }
    
    public Doctor Doctor { get; set; }
    
    public ICollection<PrescriptionMedicament> PrescriptionsMedicaments { get; set; }
}