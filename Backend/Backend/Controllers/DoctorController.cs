using Microsoft.AspNetCore.Mvc;
using Backend.Models;
using Backend.Services;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Backend.Constants;
using Backend.Models.DTOs;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = $"{Roles.Doctor},{Roles.Admin},{Roles.Patient}")]
    public class DoctorController : ControllerBase
    {
        private readonly IDoctorService _doctorService;

        public DoctorController(IDoctorService doctorService)
        {
            _doctorService = doctorService;
        }

        [HttpGet("profile/{id}")]
        public async Task<IActionResult> GetProfile(int id)
        {
            var result = await _doctorService.GetDoctorProfile(id);
            if (!result.Success)
                return BadRequest(result.ErrorMessage);

            return Ok(result.Data);
        }

        [HttpPut("profile/{id}")]
        public async Task<IActionResult> UpdateProfile(int id, UserProfileUpdateDto profileUpdate)
        {
            // Get the existing user
            var existingUser = await _doctorService.GetDoctorProfile(id);
            if (!existingUser.Success)
                return BadRequest(existingUser.ErrorMessage);

            // Update only the provided fields
            var user = existingUser.Data;
            if (profileUpdate.FirstName != null) user.FirstName = profileUpdate.FirstName;
            if (profileUpdate.LastName != null) user.LastName = profileUpdate.LastName;
            if (profileUpdate.Email != null) user.Email = profileUpdate.Email;
            if (profileUpdate.Phone != null) user.Phone = profileUpdate.Phone;
            if (profileUpdate.Address != null) user.Address = profileUpdate.Address;
            user.UpdatedAt = DateTime.UtcNow;

            var result = await _doctorService.UpdateDoctorProfile(id, user);
            if (!result.Success)
                return BadRequest(result.ErrorMessage);

            return Ok(result.Data);
        }

        [HttpGet("appointments/{doctorId}")]
        public async Task<IActionResult> GetAppointments(int doctorId)
        {
            var result = await _doctorService.GetDoctorAppointments(doctorId);
            if (!result.Success)
                return BadRequest(result.ErrorMessage);

            return Ok(result.Data);
        }

        [HttpPost("schedule")]
        public async Task<IActionResult> SetSchedule(DoctorSchedule schedule)
        {
            var result = await _doctorService.SetSchedule(schedule);
            if (!result.Success)
                return StatusCode(400, new { Message = result.ErrorMessage });

            return Ok(result.Data);
        }

        [HttpDelete("schedule/{id}")]
        public async Task<IActionResult> DeleteSchedule(int id)
        {
            var result = await _doctorService.DeleteSchedule(id);
            if (!result.Success)
                return BadRequest(result.ErrorMessage);

            return Ok(result.Data);
        }

        [HttpGet("available-time-slots/{doctorId}")]
        public async Task<IActionResult> GetAvailableTimeSlots(int doctorId, [FromQuery] DateTime date)
        {   
            Console.WriteLine($"GetAvailableTimeSlots called with doctorId: {doctorId}, date: {date}");
            
            // Ensure the date is in UTC format
            var utcDate = date.Kind == DateTimeKind.Unspecified 
                ? DateTime.SpecifyKind(date, DateTimeKind.Utc)
                : date.ToUniversalTime();
            
            var result = await _doctorService.GetAvailableTimeSlots(doctorId, utcDate);
            if (!result.Success)
            {
                Console.WriteLine($"Error in GetAvailableTimeSlots: {result.ErrorMessage}");
                return BadRequest(result.ErrorMessage);
            }

            Console.WriteLine($"Successfully retrieved {result.Data?.Count() ?? 0} time slots");
            return Ok(result.Data);
        }

        [HttpGet("schedules/{doctorId}")]
        public async Task<IActionResult> GetSchedules(int doctorId)
        {
            var result = await _doctorService.GetDoctorSchedules(doctorId);
            if (!result.Success)
                return BadRequest(result.ErrorMessage);

            return Ok(result.Data);
        }

        [HttpPost("specialty")]
        public async Task<IActionResult> AddSpecialty(DoctorSpecialty specialty)
        {
            var result = await _doctorService.AddSpecialty(specialty);
            if (!result.Success)
                return BadRequest(result.ErrorMessage);

            return Ok(result.Data);
        }

        [HttpGet("specialties/{doctorId}")]
        public async Task<IActionResult> GetSpecialties(int doctorId)
        {
            var result = await _doctorService.GetDoctorSpecialties(doctorId);
            if (!result.Success)
                return BadRequest(result.ErrorMessage);

            return Ok(result.Data);
        }

        [HttpPut("specialty/{id}")]
        public async Task<IActionResult> UpdateSpecialty(int id, DoctorSpecialty specialty)
        {
            var result = await _doctorService.UpdateSpecialty(id, specialty);
            if (!result.Success)
                return BadRequest(result.ErrorMessage);

            return Ok(result.Data);
        }

        [HttpDelete("specialty/{id}")]
        public async Task<IActionResult> RemoveSpecialty(int id)
        {
            var result = await _doctorService.RemoveSpecialty(id);
            if (!result.Success)
                return BadRequest(result.ErrorMessage);

            return Ok(result.Data);
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchDoctors([FromQuery] string specialty, [FromQuery] string name)
        {
            var result = await _doctorService.SearchDoctors(specialty, name);
            if (!result.Success)
                return BadRequest(result.ErrorMessage);

            return Ok(result.Data);
        }

        [HttpGet("available")]
        public async Task<IActionResult> GetAvailableDoctors([FromQuery] DateTime date, [FromQuery] TimeSpan time)
        {
            var result = await _doctorService.GetAvailableDoctors(date, time);
            if (!result.Success)
                return BadRequest(result.ErrorMessage);

            return Ok(result.Data);
        }
    }
} 