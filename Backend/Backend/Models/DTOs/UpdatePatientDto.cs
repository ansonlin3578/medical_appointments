using System.ComponentModel.DataAnnotations;

namespace Backend.Models.DTOs
{
    public class UpdatePatientDto(
        [Required]
        [StringLength(100)]
        string name,
        [Phone]
        string? phone = null,
        DateTime? birthDate = null,
        string? medicalHistory = null,
        string? address = null)
    {
        public string Name { get; set; } = name;
        public string? Phone { get; set; } = phone;
        public DateTime? BirthDate { get; set; } = birthDate;
        public string? MedicalHistory { get; set; } = medicalHistory;
        public string? Address { get; set; } = address;
    }
} 