using Microsoft.AspNetCore.Mvc;
using Backend.Models;
using Backend.Services;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Patient,Admin")]
    public class PatientController : ControllerBase
    {
        private readonly IPatientService _patientService;

        public PatientController(IPatientService patientService)
        {
            _patientService = patientService;
        }

        [HttpPost("profile")]
        public async Task<IActionResult> CreateProfile(Patient patient)
        {
            var result = await _patientService.CreatePatient(patient);
            if (!result.Success)
                return BadRequest(result.ErrorMessage);

            return Ok(result.Data);
        }

        [HttpGet("profile/{id}")]
        public async Task<IActionResult> GetProfile(int id)
        {
            var result = await _patientService.GetPatientProfile(id);
            if (!result.Success)
                return BadRequest(result.ErrorMessage);

            return Ok(result.Data);
        }

        [HttpPut("profile/{id}")]
        public async Task<IActionResult> UpdateProfile(int id, Patient patient)
        {
            var result = await _patientService.UpdatePatientProfile(id, patient);
            if (!result.Success)
                return BadRequest(result.ErrorMessage);

            return Ok(result.Data);
        }

        [HttpGet("appointments/{patientId}")]
        public async Task<IActionResult> GetAppointments(int patientId)
        {
            var result = await _patientService.GetPatientAppointments(patientId);
            if (!result.Success)
                return BadRequest(result.ErrorMessage);

            return Ok(result.Data);
        }

        [HttpPost("appointments/cancel/{appointmentId}")]
        public async Task<IActionResult> CancelAppointment(int appointmentId)
        {
            var result = await _patientService.CancelAppointment(appointmentId);
            if (!result.Success)
                return BadRequest(result.ErrorMessage);

            return Ok(result.Data);
        }

        [HttpGet("available-time-slots/{doctorId}")]
        public async Task<IActionResult> GetAvailableTimeSlots(int doctorId, [FromQuery] DateTime date)
        {
            var result = await _patientService.GetAvailableTimeSlots(doctorId, date);
            if (!result.Success)
                return BadRequest(result.ErrorMessage);

            return Ok(result.Data);
        }

        [HttpGet("check-conflict/{patientId}")]
        public async Task<IActionResult> CheckAppointmentConflict(
            int patientId,
            [FromQuery] DateTime date,
            [FromQuery] TimeSpan startTime,
            [FromQuery] TimeSpan endTime)
        {
            var result = await _patientService.CheckAppointmentConflict(patientId, date, startTime, endTime);
            if (!result.Success)
                return BadRequest(result.ErrorMessage);

            return Ok(result.Data);
        }
    }
} 