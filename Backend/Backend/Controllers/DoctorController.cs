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
    [Authorize(Roles = "Doctor,Admin")]
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
        public async Task<IActionResult> UpdateProfile(int id, Doctor doctor)
        {
            var result = await _doctorService.UpdateDoctorProfile(id, doctor);
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
            var result = await _doctorService.GetAvailableTimeSlots(doctorId, date);
            if (!result.Success)
                return BadRequest(result.ErrorMessage);

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