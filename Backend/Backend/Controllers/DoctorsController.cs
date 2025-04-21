using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Backend.Models;
using Backend.Services;

namespace Backend.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class DoctorsController : ControllerBase
    {
        private readonly IDoctorService _doctorService;

        public DoctorsController(IDoctorService doctorService)
        {
            _doctorService = doctorService;
        }

        // 排班管理
        [HttpPost("schedules")]
        public async Task<IActionResult> SetSchedule([FromBody] DoctorSchedule schedule)
        {
            var result = await _doctorService.SetSchedule(schedule);
            
            if (!result.Success)
            {
                return result.ErrorCode switch
                {
                    "DOCTOR_NOT_FOUND" => NotFound(new { Message = result.ErrorMessage }),
                    "INVALID_TIME_RANGE" => BadRequest(new { Message = result.ErrorMessage }),
                    "SCHEDULE_OVERLAP" => BadRequest(new { Message = result.ErrorMessage }),
                    "SCHEDULE_SET_ERROR" => StatusCode(500, new { Message = result.ErrorMessage }),
                    _ => StatusCode(500, new { Message = "An unexpected error occurred" })
                };
            }

            return Ok(result.Data);
        }

        [HttpGet("schedules/{doctorId}")]
        public async Task<IActionResult> GetDoctorSchedules(int doctorId)
        {
            var result = await _doctorService.GetDoctorSchedules(doctorId);
            
            if (!result.Success)
            {
                return result.ErrorCode switch
                {
                    "SCHEDULES_RETRIEVAL_ERROR" => StatusCode(500, new { Message = result.ErrorMessage }),
                    _ => StatusCode(500, new { Message = "An unexpected error occurred" })
                };
            }

            return Ok(result.Data);
        }

        [HttpDelete("schedules/{scheduleId}")]
        public async Task<IActionResult> DeleteSchedule(int scheduleId)
        {
            var result = await _doctorService.DeleteSchedule(scheduleId);
            
            if (!result.Success)
            {
                return result.ErrorCode switch
                {
                    "SCHEDULE_NOT_FOUND" => NotFound(new { Message = result.ErrorMessage }),
                    "SCHEDULE_DELETION_ERROR" => StatusCode(500, new { Message = result.ErrorMessage }),
                    _ => StatusCode(500, new { Message = "An unexpected error occurred" })
                };
            }

            return Ok(new { Message = "Schedule deleted successfully" });
        }

        // 專業領域管理
        [HttpPost("specialties")]
        public async Task<IActionResult> AddSpecialty([FromBody] DoctorSpecialty specialty)
        {
            var result = await _doctorService.AddSpecialty(specialty);
            
            if (!result.Success)
            {
                return result.ErrorCode switch
                {
                    "DOCTOR_NOT_FOUND" => NotFound(new { Message = result.ErrorMessage }),
                    "SPECIALTY_ADDITION_ERROR" => StatusCode(500, new { Message = result.ErrorMessage }),
                    _ => StatusCode(500, new { Message = "An unexpected error occurred" })
                };
            }

            return Ok(result.Data);
        }

        [HttpGet("specialties/{doctorId}")]
        public async Task<IActionResult> GetDoctorSpecialties(int doctorId)
        {
            var result = await _doctorService.GetDoctorSpecialties(doctorId);
            
            if (!result.Success)
            {
                return result.ErrorCode switch
                {
                    "SPECIALTIES_RETRIEVAL_ERROR" => StatusCode(500, new { Message = result.ErrorMessage }),
                    _ => StatusCode(500, new { Message = "An unexpected error occurred" })
                };
            }

            return Ok(result.Data);
        }

        [HttpDelete("specialties/{specialtyId}")]
        public async Task<IActionResult> RemoveSpecialty(int specialtyId)
        {
            var result = await _doctorService.RemoveSpecialty(specialtyId);
            
            if (!result.Success)
            {
                return result.ErrorCode switch
                {
                    "SPECIALTY_NOT_FOUND" => NotFound(new { Message = result.ErrorMessage }),
                    "SPECIALTY_REMOVAL_ERROR" => StatusCode(500, new { Message = result.ErrorMessage }),
                    _ => StatusCode(500, new { Message = "An unexpected error occurred" })
                };
            }

            return Ok(new { Message = "Specialty removed successfully" });
        }

        // 醫生信息管理
        [HttpPut("profile/{doctorId}")]
        public async Task<IActionResult> UpdateDoctorProfile(int doctorId, [FromBody] User doctor)
        {
            var result = await _doctorService.UpdateDoctorProfile(doctorId, doctor);
            
            if (!result.Success)
            {
                return result.ErrorCode switch
                {
                    "DOCTOR_NOT_FOUND" => NotFound(new { Message = result.ErrorMessage }),
                    "PROFILE_UPDATE_ERROR" => StatusCode(500, new { Message = result.ErrorMessage }),
                    _ => StatusCode(500, new { Message = "An unexpected error occurred" })
                };
            }

            return Ok(result.Data);
        }

        [HttpGet("profile/{doctorId}")]
        public async Task<IActionResult> GetDoctorProfile(int doctorId)
        {
            var result = await _doctorService.GetDoctorProfile(doctorId);
            
            if (!result.Success)
            {
                return result.ErrorCode switch
                {
                    "DOCTOR_NOT_FOUND" => NotFound(new { Message = result.ErrorMessage }),
                    "PROFILE_RETRIEVAL_ERROR" => StatusCode(500, new { Message = result.ErrorMessage }),
                    _ => StatusCode(500, new { Message = "An unexpected error occurred" })
                };
            }

            return Ok(result.Data);
        }

        // 搜索和過濾
        [HttpGet("search")]
        public async Task<IActionResult> SearchDoctors([FromQuery] string specialty, [FromQuery] string name)
        {
            var result = await _doctorService.SearchDoctors(specialty, name);
            
            if (!result.Success)
            {
                return result.ErrorCode switch
                {
                    "DOCTOR_SEARCH_ERROR" => StatusCode(500, new { Message = result.ErrorMessage }),
                    _ => StatusCode(500, new { Message = "An unexpected error occurred" })
                };
            }

            return Ok(result.Data);
        }

        [HttpGet("available")]
        public async Task<IActionResult> GetAvailableDoctors([FromQuery] DateTime date, [FromQuery] TimeSpan time)
        {
            var result = await _doctorService.GetAvailableDoctors(date, time);
            
            if (!result.Success)
            {
                return result.ErrorCode switch
                {
                    "AVAILABLE_DOCTORS_RETRIEVAL_ERROR" => StatusCode(500, new { Message = result.ErrorMessage }),
                    _ => StatusCode(500, new { Message = "An unexpected error occurred" })
                };
            }

            return Ok(result.Data);
        }
    }
}