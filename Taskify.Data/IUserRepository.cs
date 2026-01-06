using System;
using System.Threading.Tasks;
using Taskify.Model;

namespace Taskify.Data
{
    public interface IUserRepository
    {
        Task<User?> GetByEmailAsync(string email);
        Task CreateAsync(User user);
    }
}
