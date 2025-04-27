using System.ComponentModel.DataAnnotations;

namespace Backend.Models
{
    public class User(int id, string username, string email, string passwordHash, string firstName, string lastName, string role)
    {
        [Key]
        public int Id { get; set; } = id;

        [Required]
        [StringLength(50)]
        public string Username { get; set; } = username;

        [Required]
        [StringLength(100)]
        public string Email { get; set; } = email;

        [Required]
        public string PasswordHash { get; set; } = passwordHash;

        [Required]
        [StringLength(50)]
        public string FirstName { get; set; } = firstName;

        [Required]
        [StringLength(50)]
        public string LastName { get; set; } = lastName;

        [Phone]
        public string? Phone { get; set; }

        [StringLength(200)]
        public string? Address { get; set; }

        [Required]
        public string Role { get; set; } = role ?? throw new ArgumentNullException(nameof(role), "Role cannot be null");

        // 導航屬性
        public ICollection<DoctorSchedule> DoctorSchedules { get; set; } = new List<DoctorSchedule>();
        public ICollection<DoctorSpecialty> DoctorSpecialties { get; set; } = new List<DoctorSpecialty>();

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
} 