using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Security.Cryptography;
using Backend.Data;
using Backend.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Logging;
using Backend.Constants;

namespace Backend.Services
{
    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthService> _logger;

        public AuthService(ApplicationDbContext context, IConfiguration configuration, ILogger<AuthService> logger)
        {
            _context = context;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<ServiceResult<string>> Register(User user, string password)
        {
            try
            {
                _logger.LogInformation("Starting user registration for {Username}", user.Username);

                if (await _context.Users.AnyAsync(u => u.Username == user.Username))
                {
                    _logger.LogWarning("Username {Username} already exists", user.Username);
                    return ServiceResult<string>.ErrorResult("Username already exists", "USERNAME_EXISTS");
                }

                if (await _context.Users.AnyAsync(u => u.Email == user.Email))
                {
                    _logger.LogWarning("Email {Email} already exists", user.Email);
                    return ServiceResult<string>.ErrorResult("Email already exists", "EMAIL_EXISTS");
                }

                user.PasswordHash = HashPassword(password);
                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                _logger.LogInformation("User {Username} registered successfully", user.Username);

                if (user.Role.ToLower() == "patient")
                {
                    var patient = new Patient
                    {
                        UserId = user.Id,
                        Name = $"{user.FirstName} {user.LastName}",
                        Phone = user.Phone,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    };

                    _context.Patients.Add(patient);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("Patient record created for user {Username}", user.Username);
                }
                else if (user.Role.ToLower() == "doctor")
                {
                    var doctorSpecialty = new DoctorSpecialty
                    {
                        DoctorId = user.Id,
                        Specialty = "General Medicine", // Default specialty
                        Description = "General medical practice",
                        YearsOfExperience = 0
                    };

                    _context.DoctorSpecialties.Add(doctorSpecialty);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("Doctor specialty record created for user {Username}", user.Username);
                }

                var token = GenerateJwtToken(user);
                return ServiceResult<string>.SuccessResult(token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during registration for user {Username}: {Message}", user.Username, ex.Message);
                return ServiceResult<string>.ErrorResult($"An error occurred during registration: {ex.Message}", "REGISTRATION_ERROR");
            }
        }

        public async Task<ServiceResult<string>> Login(string username, string password)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
                if (user == null || !VerifyPassword(password, user.PasswordHash))
                    return ServiceResult<string>.ErrorResult("Invalid username or password", "INVALID_CREDENTIALS");

                var token = GenerateJwtToken(user);
                return ServiceResult<string>.SuccessResult(token);
            }
            catch (Exception ex)
            {
                return ServiceResult<string>.ErrorResult("An error occurred during login", "LOGIN_ERROR");
            }
        }

        public async Task<ServiceResult<User>> GetUserById(int id)
        {
            try
            {
                var user = await _context.Users.FindAsync(id);
                if (user == null)
                    return ServiceResult<User>.ErrorResult($"User with ID {id} not found", "USER_NOT_FOUND");

                return ServiceResult<User>.SuccessResult(user);
            }
            catch (Exception ex)
            {
                return ServiceResult<User>.ErrorResult("An error occurred while retrieving user information", "USER_RETRIEVAL_ERROR");
            }
        }

        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashedBytes);
            }
        }

        private bool VerifyPassword(string password, string hash)
        {
            return HashPassword(password) == hash;
        }

        private string GenerateJwtToken(User user)
        {
            // Normalize the role to match the expected case
            string normalizedRole = user.Role.ToLower() switch
            {
                "patient" => "Patient",
                "doctor" => "Doctor",
                "admin" => "Admin",
                _ => user.Role
            };

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, normalizedRole)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}