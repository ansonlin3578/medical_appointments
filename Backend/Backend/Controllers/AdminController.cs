using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Backend.Services;
using Backend.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IPatientService _patientService;

        public AdminController(IUserService userService, IPatientService patientService)
        {
            _userService = userService;
            _patientService = patientService;
        }

        [HttpGet("users")]
        public async Task<ActionResult<IEnumerable<User>>> GetAllUsers()
        {
            var users = await _userService.GetAllUsers();
            return Ok(users);
        }

        [HttpPost("move-to-patient/{userId}")]
        public async Task<IActionResult> MoveToPatientTable(int userId)
        {
            try
            {
                var user = await _userService.GetUserById(userId);
                if (user == null)
                {
                    return NotFound("User not found");
                }

                if (user.Role != "Patient")
                {
                    return BadRequest("Only patient users can be moved to the patient table");
                }

                var patient = await _patientService.GetPatientByUserId(userId);
                if (patient != null)
                {
                    return BadRequest("User is already in the patient table");
                }

                await _patientService.CreatePatientFromUser(user);
                return Ok(new { message = "User successfully moved to patient table" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred", error = ex.Message });
            }
        }
    }
} 