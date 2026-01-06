using System;

namespace Taskify.Model
{
    public enum Role { Admin, User}
    public class User
    {
        public Guid Id { get; set; }
        public string UserEmail { get; set; } = default!;
        public string Password { get; set; } = default!; // store hashed password in production
        public string Role { get; set; } = default!;
        public DateTime CreatedAt { get; set; }
    }
}
