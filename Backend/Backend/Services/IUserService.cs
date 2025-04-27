using System.Collections.Generic;
using System.Threading.Tasks;
using Backend.Models;

namespace Backend.Services
{
    public interface IUserService
    {
        Task<User> GetUserById(int id);
        Task<IEnumerable<User>> GetAllUsers();
    }
} 