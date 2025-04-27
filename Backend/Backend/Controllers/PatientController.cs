using Microsoft.AspNetCore.Mvc;
using Backend.Models;
using Backend.Services;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Backend.Constants;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = $"{Roles.Patient},{Roles.Admin}")]
    public class PatientController : ControllerBase
    {
        private readonly IPatientService _patientService;
        private readonly ILogger<PatientController> _logger;

        public PatientController(IPatientService patientService, ILogger<PatientController> logger)
        {
            _patientService = patientService;
            _logger = logger;
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
        public async Task<IActionResult> UpdateProfile(int id, UpdatePatientDto patientDto)
        {
            var result = await _patientService.UpdatePatientProfile(id, patientDto);
            if (!result.Success)
                return BadRequest(result.ErrorMessage);

            return Ok(result.Data);
        }

        [HttpGet("appointments")]
        public async Task<IActionResult> GetPatientAppointments()
        {
            _logger.LogInformation("GetPatientAppointments called");
            _logger.LogInformation($"User claims: {string.Join(", ", User.Claims.Select(c => $"{c.Type}: {c.Value}"))}");
            _logger.LogInformation($"User identity: {User.Identity?.Name}, IsAuthenticated: {User.Identity?.IsAuthenticated}");
            
            try
            {
                // 從 JWT token 中獲取用戶 ID
                var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
                if (userIdClaim == null)
                {
                    _logger.LogError("User ID claim not found in token");
                    return BadRequest("User ID not found in token");
                }

                var userId = int.Parse(userIdClaim.Value);
                _logger.LogInformation($"Using userId from token: {userId}");

                var result = await _patientService.GetPatientAppointments(userId);
                if (!result.Success)
                {
                    _logger.LogError($"Error in GetPatientAppointments: {result.ErrorMessage}");
                    return BadRequest(result.ErrorMessage);
                }

                _logger.LogInformation($"Successfully retrieved {result.Data?.Count() ?? 0} appointments");
                return Ok(result.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception in GetPatientAppointments: {ex.Message}");
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        [HttpGet("available-time-slots/{doctorId}")]
        public async Task<IActionResult> GetAvailableTimeSlots(int doctorId, [FromQuery] DateTime date)
        {
            var result = await _patientService.GetAvailableTimeSlots(doctorId, date);
            if (!result.Success)
                return BadRequest(result.ErrorMessage);

            return Ok(result.Data);
        }

        [HttpGet("appointments/{patientId}")]
        public async Task<IActionResult> GetPatientAppointments(int patientId)
        {
            var result = await _patientService.GetPatientAppointments(patientId);
            if (!result.Success)
                return BadRequest(result.ErrorMessage);

            return Ok(result.Data);
        }

        [HttpGet("doctors")]
        public async Task<IActionResult> GetAllDoctors()
        {
            var result = await _patientService.GetAllDoctors();
            if (!result.Success)
                return BadRequest(result.ErrorMessage);

            return Ok(result.Data);
        }
    }
} 