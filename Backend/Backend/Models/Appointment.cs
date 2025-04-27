using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Backend.Models;

public class Appointment(int id, int patientId, int doctorId, DateTime appointmentDate, TimeSpan startTime, TimeSpan endTime, string status)
{
    [Key]
    public int Id { get; set; } = id;

    [Required(ErrorMessage = "PatientId is required")]
    public int PatientId { get; set; } = patientId;

    [Required(ErrorMessage = "DoctorId is required")]
    public int DoctorId { get; set; } = doctorId;

    [Required(ErrorMessage = "AppointmentDate is required")]
    [DataType(DataType.Date)]
    [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
    public DateTime AppointmentDate { get; set; } = appointmentDate;

    [Required(ErrorMessage = "StartTime is required")]
    [JsonConverter(typeof(TimeSpanConverter))]
    public TimeSpan StartTime { get; set; } = startTime;

    [Required(ErrorMessage = "EndTime is required")]
    [JsonConverter(typeof(TimeSpanConverter))]
    public TimeSpan EndTime { get; set; } = endTime;

    [Required(ErrorMessage = "Status is required")]
    public string Status { get; set; } = status ?? throw new ArgumentNullException(nameof(status), "Status cannot be null");

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

public class TimeSpanConverter : JsonConverter<TimeSpan>
{
    public override TimeSpan Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var timeString = reader.GetString();
        if (TimeSpan.TryParse(timeString, out TimeSpan time))
        {
            return time;
        }
        throw new JsonException($"Invalid time format: {timeString}. Expected format: HH:mm:ss");
    }

    public override void Write(Utf8JsonWriter writer, TimeSpan value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString(@"hh\:mm\:ss"));
    }
}