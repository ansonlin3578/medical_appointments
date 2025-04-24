using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Backend.Models;
using Backend.Services;
using System.Text.Json;

namespace Backend.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class AppointmentController : ControllerBase
    {
        private readonly IAppointmentService _appointmentService;

        public AppointmentController(IAppointmentService appointmentService)
        {
            _appointmentService = appointmentService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateAppointment([FromBody] Appointment appointment)
        {
            Console.WriteLine($"Received appointment data: {JsonSerializer.Serialize(appointment)}");
            
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();
                
                Console.WriteLine($"Model validation errors: {string.Join(", ", errors)}");
                Console.WriteLine($"ModelState: {JsonSerializer.Serialize(ModelState)}");
                
                return BadRequest(new { 
                    Message = "Invalid appointment data", 
                    Errors = errors,
                    ModelState = ModelState
                });
            }

            try
            {
                // Ensure DateTime values are in UTC
                appointment.AppointmentDate = appointment.AppointmentDate.ToUniversalTime();
                appointment.CreatedAt = DateTime.UtcNow;
                appointment.UpdatedAt = DateTime.UtcNow;

                // Convert string times to TimeSpan
                if (TimeSpan.TryParse(appointment.StartTime.ToString(), out TimeSpan startTime) &&
                    TimeSpan.TryParse(appointment.EndTime.ToString(), out TimeSpan endTime))
                {
                    appointment.StartTime = startTime;
                    appointment.EndTime = endTime;
                }
                else
                {
                    Console.WriteLine($"Failed to parse times: StartTime={appointment.StartTime}, EndTime={appointment.EndTime}");
                    return BadRequest(new { Message = "Invalid time format. Please use HH:mm:ss format" });
                }

                var result = await _appointmentService.CreateAppointment(appointment);
                
                if (!result.Success)
                {
                    Console.WriteLine($"Appointment creation failed: {result.ErrorMessage} (Code: {result.ErrorCode})");
                    return result.ErrorCode switch
                    {
                        "TIME_SLOT_UNAVAILABLE" => BadRequest(new { Message = result.ErrorMessage }),
                        "DOCTOR_UNAVAILABLE" => BadRequest(new { Message = result.ErrorMessage }),
                        "APPOINTMENT_CREATION_ERROR" => StatusCode(500, new { Message = result.ErrorMessage }),
                        _ => StatusCode(500, new { Message = "An unexpected error occurred" })
                    };
                }

                Console.WriteLine($"Appointment created successfully: ID={result.Data?.Id}");
                return Ok(result.Data);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception in CreateAppointment: {ex}");
                return StatusCode(500, new { Message = "An error occurred while creating the appointment", Error = ex.Message });
            }
        }

        [HttpGet("patient/{patientId}")]
        public async Task<IActionResult> GetAppointmentsByPatientId(int patientId)
        {
            var result = await _appointmentService.GetPatientAppointments(patientId);
            
            if (!result.Success)
            {
                return result.ErrorCode switch
                {
                    "PATIENT_APPOINTMENTS_RETRIEVAL_ERROR" => StatusCode(500, new { Message = result.ErrorMessage }),
                    _ => StatusCode(500, new { Message = "An unexpected error occurred" })
                };
            }

            return Ok(result.Data);
        }

        [HttpGet("doctor/{doctorId}")]
        public async Task<IActionResult> GetDoctorAppointments(int doctorId)
        {
            var result = await _appointmentService.GetDoctorAppointments(doctorId);
            
            if (!result.Success)
            {
                return result.ErrorCode switch
                {
                    "DOCTOR_APPOINTMENTS_RETRIEVAL_ERROR" => StatusCode(500, new { Message = result.ErrorMessage }),
                    _ => StatusCode(500, new { Message = "An unexpected error occurred" })
                };
            }

            return Ok(result.Data);
        }

        [HttpPost("{appointmentId}/cancel")]
        public async Task<IActionResult> CancelAppointment(int appointmentId)
        {
            var result = await _appointmentService.CancelAppointment(appointmentId);
            
            if (!result.Success)
            {
                return result.ErrorCode switch
                {
                    "APPOINTMENT_NOT_FOUND" => NotFound(new { Message = result.ErrorMessage }),
                    "APPOINTMENT_CANCELLATION_ERROR" => StatusCode(500, new { Message = result.ErrorMessage }),
                    _ => StatusCode(500, new { Message = "An unexpected error occurred" })
                };
            }

            return Ok(new { Message = "Appointment cancelled successfully" });
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateAppointment(int id, [FromBody] Appointment updatedAppointment)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _appointmentService.UpdateAppointment(id, updatedAppointment);
            
            if (!result.Success)
            {
                return result.ErrorCode switch
                {
                    "APPOINTMENT_NOT_FOUND" => NotFound(new { message = result.ErrorMessage }),
                    "TIME_SLOT_UNAVAILABLE" => BadRequest(new { message = result.ErrorMessage }),
                    _ => StatusCode(500, new { message = result.ErrorMessage })
                };
            }

            return Ok(result.Data);
        }
    }
} 