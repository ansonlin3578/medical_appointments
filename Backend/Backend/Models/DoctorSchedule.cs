using System.ComponentModel.DataAnnotations;

namespace Backend.Models
{
    public class DoctorSchedule
    {
        public int Id { get; set; }
        
        [Required]
        public int DoctorId { get; set; }
        
        [Required]
        public DayOfWeek DayOfWeek { get; set; }
        
        [Required]
        public TimeSpan StartTime { get; set; }
        
        [Required]
        public TimeSpan EndTime { get; set; }
        
        [Required]
        public bool IsAvailable { get; set; }
        
        public string Notes { get; set; } = string.Empty;
    }
} 