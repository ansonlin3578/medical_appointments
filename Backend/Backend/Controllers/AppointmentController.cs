using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Backend.Models;
using Backend.Services;

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
            var result = await _appointmentService.CreateAppointment(appointment);
            
            if (!result.Success)
            {
                return result.ErrorCode switch
                {
                    "TIME_SLOT_UNAVAILABLE" => BadRequest(new { Message = result.ErrorMessage }),
                    "DOCTOR_UNAVAILABLE" => BadRequest(new { Message = result.ErrorMessage }),
                    "APPOINTMENT_CREATION_ERROR" => StatusCode(500, new { Message = result.ErrorMessage }),
                    _ => StatusCode(500, new { Message = "An unexpected error occurred" })
                };
            }

            return Ok(result.Data);
        }

        [HttpGet("patient/{patientId}")]
        public async Task<IActionResult> GetPatientAppointments(int patientId)
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