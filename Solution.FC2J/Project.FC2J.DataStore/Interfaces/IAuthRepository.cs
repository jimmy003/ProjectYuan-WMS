using Project.FC2J.Models;
using Project.FC2J.Models.Dtos;
using System.Threading.Tasks;
using Project.FC2J.Models.User;

namespace Project.FC2J.DataStore.Interfaces
{
    public interface IAuthRepository : IRepository<User> 
    {
        Task<User> Register(User user, string password);
        Task<User> Login(string userName, string password);
        Task<User> Login(UserForLoginDto value);
        Task<User> ValidateUser(UserForLoginDto value);
        Task<UserForLoginDto> GetHash(UserForLoginDto value);
        Task<bool> UserExists(string userName);
        Task<User> GetUserByUserNameAsync(string userName);
    }
}
