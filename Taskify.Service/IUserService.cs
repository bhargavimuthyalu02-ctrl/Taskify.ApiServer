using System.Threading.Tasks;
using Taskify.Model;

namespace Taskify.Service
{
    public interface IUserService
    {
        Task<User> RegisterAsync(string email, string password, string role);
        Task<string?> LoginAsync(string email, string password);
    }
}
