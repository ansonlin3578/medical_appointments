namespace Backend.Models;

public class Appointment
{
    public Guid Id { get; set; }
    public string Name { get; set; } = "";
    public DateOnly Date { get; set; }        // e.g., 2025-04-22
    public TimeSpan StartTime { get; set; }   // e.g., 09:00:00
    public TimeSpan EndTime { get; set; }     // e.g., 10:00:00
    public Guid DoctorId { get; set; }
}