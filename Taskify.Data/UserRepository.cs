using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Taskify.Model;

namespace Taskify.Data
{
    public class UserRepository : IUserRepository
    {
        private readonly TaskDbContext _db;

        public UserRepository(TaskDbContext db)
        {
            _db = db;
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _db.Users.FirstOrDefaultAsync(u => u.UserEmail == email);
        }

        public async Task CreateAsync(User user)
        {
            user.Id = user.Id == Guid.Empty ? Guid.NewGuid() : user.Id;
            await _db.Users.AddAsync(user);
            await _db.SaveChangesAsync();
        }
    }
}
