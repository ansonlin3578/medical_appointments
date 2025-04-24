using System;

namespace Backend.Models
{
    public class TimeSlot
    {
        public int Id { get; set; }
        public TimeSpan Time { get; set; }
        public bool IsAvailable { get; set; }
    }
} 