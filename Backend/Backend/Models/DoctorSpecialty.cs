using System.ComponentModel.DataAnnotations;

namespace Backend.Models
{
    public class DoctorSpecialty
    {
        public int Id { get; set; }
        
        [Required]
        public int DoctorId { get; set; }
        public User Doctor { get; set; }
        
        [Required]
        [StringLength(100)]
        public string Specialty { get; set; }
        
        [StringLength(500)]
        public string Description { get; set; }
        
        [Required]
        public int YearsOfExperience { get; set; }
    }
} 