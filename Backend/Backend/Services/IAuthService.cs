using Backend.Models;

namespace Backend.Services
{
    public interface IAuthService
    {
        Task<ServiceResult<string>> Register(User user, string password);
        Task<ServiceResult<string>> Login(string username, string password);
        Task<ServiceResult<User>> GetUserById(int id);
    }
} 