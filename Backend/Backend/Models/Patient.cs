using System.ComponentModel.DataAnnotations;

namespace Backend.Models
{
    public class Patient(int id, int userId, string name)
    {
        [Key]
        public int Id { get; set; } = id;

        [Required]
        public int UserId { get; set; } = userId;
        
        public User User { get; set; } = null!;

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = name;

        public DateTime? BirthDate { get; set; }

        public string? MedicalHistory { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
} 