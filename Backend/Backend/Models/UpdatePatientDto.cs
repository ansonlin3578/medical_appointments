using System.ComponentModel.DataAnnotations;

namespace Backend.Models
{
    public class UpdatePatientDto
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Phone]
        public string? Phone { get; set; }

        public DateTime? BirthDate { get; set; }

        public string? MedicalHistory { get; set; }

        public string? Address { get; set; }
    }
} 