using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Backend.Data;
using Backend.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace Backend.Services
{
    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthService(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<ServiceResult<string>> Register(User user, string password)
        {
            try
            {
                if (await _context.Users.AnyAsync(u => u.Username == user.Username))
                    return ServiceResult<string>.ErrorResult("Username already exists", "USERNAME_EXISTS");

                if (await _context.Users.AnyAsync(u => u.Email == user.Email))
                    return ServiceResult<string>.ErrorResult("Email already exists", "EMAIL_EXISTS");

                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(password);
                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                var token = GenerateJwtToken(user);
                return ServiceResult<string>.SuccessResult(token);
            }
            catch (Exception ex)
            {
                return ServiceResult<string>.ErrorResult("An error occurred during registration", "REGISTRATION_ERROR");
            }
        }

        public async Task<ServiceResult<string>> Login(string username, string password)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
                if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
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

        private string GenerateJwtToken(User user)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role)
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