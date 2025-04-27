using System.ComponentModel.DataAnnotations;

namespace Backend.Models
{
    public class DoctorSpecialty(int id, int doctorId, string specialty, string description, int yearsOfExperience)
    {
        public int Id { get; set; } = id;
        
        [Required]
        public int DoctorId { get; set; } = doctorId;
        
        [Required]
        [StringLength(100)]
        public string Specialty { get; set; } = specialty;
        
        [StringLength(500)]
        public string Description { get; set; } = description;
        
        [Required]
        public int YearsOfExperience { get; set; } = yearsOfExperience;
    }
} 