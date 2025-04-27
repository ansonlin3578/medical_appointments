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
        private readonly ILogger<AppointmentController> _logger;

        public AppointmentController(IAppointmentService appointmentService, ILogger<AppointmentController> logger)
        {
            _appointmentService = appointmentService;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> CreateAppointment([FromBody] Appointment appointment)
        {
            _logger.LogInformation($"Received appointment data: {JsonSerializer.Serialize(appointment)}");

            try
            {
                var result = await _appointmentService.CreateAppointment(appointment);
                
                if (!result.Success)
                {
                    _logger.LogWarning($"Appointment creation failed: {result.ErrorMessage} (Code: {result.ErrorCode})");
                    return result.ErrorCode switch
                    {
                        "TIME_SLOT_NOT_AVAILABLE" => BadRequest(new { Message = result.ErrorMessage }),
                        "VALIDATION_ERROR" => BadRequest(new { Message = result.ErrorMessage }),
                        "INVALID_TIME_FORMAT" => BadRequest(new { Message = result.ErrorMessage }),
                        "APPOINTMENT_CREATION_ERROR" => StatusCode(500, new { Message = result.ErrorMessage }),
                        _ => StatusCode(400, new { Message = "An unexpected error occurred" })
                    };
                }

                _logger.LogInformation($"Appointment created successfully: ID={result.Data?.Id}");
                return Ok(result.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception in CreateAppointment");
                return StatusCode(500, new { Message = "An error occurred while creating the appointment" });
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