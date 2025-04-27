using System.ComponentModel.DataAnnotations;

namespace Backend.Models
{
    public class DoctorSchedule(int id, int doctorId, DayOfWeek dayOfWeek, TimeSpan startTime, TimeSpan endTime, bool isAvailable)
    {
        public int Id { get; set; } = id;
        
        [Required]
        public int DoctorId { get; set; } = doctorId;
        
        [Required]
        public DayOfWeek DayOfWeek { get; set; } = dayOfWeek;
        
        [Required]
        public TimeSpan StartTime { get; set; } = startTime;
        
        [Required]
        public TimeSpan EndTime { get; set; } = endTime;
        
        [Required]
        public bool IsAvailable { get; set; } = isAvailable;
        
        public string Notes { get; set; } = string.Empty;
    }
} 