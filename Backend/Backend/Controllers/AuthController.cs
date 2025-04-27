using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Backend.Models;
using Backend.Services;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            var user = new User(
                id: 0, // Will be set by the database
                username: request.Username,
                email: request.Email,
                passwordHash: "", // Will be set by AuthService
                firstName: request.FirstName,
                lastName: request.LastName,
                role: request.Role
            );

            var result = await _authService.Register(user, request.Password);
            
            if (!result.Success)
            {
                return result.ErrorCode switch
                {
                    "USERNAME_EXISTS" => BadRequest(new { Message = result.ErrorMessage }),
                    "EMAIL_EXISTS" => BadRequest(new { Message = result.ErrorMessage }),
                    _ => StatusCode(500, new { Message = result.ErrorMessage })
                };
            }

            return Ok(new { Token = result.Data });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var result = await _authService.Login(request.Username, request.Password);
            
            if (!result.Success)
            {
                return result.ErrorCode switch
                {
                    "INVALID_CREDENTIALS" => Unauthorized(new { Message = result.ErrorMessage }),
                    _ => StatusCode(500, new { Message = result.ErrorMessage })
                };
            }

            return Ok(new { 
                token = result.Data,
                message = "Login successful"
            });
        }

        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> GetCurrentUser()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var result = await _authService.GetUserById(userId);
            
            if (!result.Success)
            {
                return result.ErrorCode switch
                {
                    "USER_NOT_FOUND" => NotFound(new { Message = result.ErrorMessage }),
                    _ => StatusCode(500, new { Message = result.ErrorMessage })
                };
            }

            return Ok(result.Data);
        }
    }

    public class RegisterRequest
    {
        public required string Username { get; set; }
        public required string Email { get; set; }
        public required string Password { get; set; }
        public required string Role { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
    }

    public class LoginRequest
    {
        public required string Username { get; set; }
        public required string Password { get; set; }
    }
} 