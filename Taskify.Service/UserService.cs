using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Taskify.Data;
using Taskify.Model;

namespace Taskify.Service
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _repo;
        private readonly IConfiguration _config;

        public UserService(IUserRepository repo, IConfiguration config)
        {
            _repo = repo;
            _config = config;
        }

        public async Task<User> RegisterAsync(string email, string password, string role)
        {
            var existing = await _repo.GetByEmailAsync(email);
            if (existing != null) throw new InvalidOperationException("User already exists");

            var user = new User
            {
                Id = Guid.NewGuid(),
                UserEmail = email,
                Password = HashPassword(password),
                CreatedAt = DateTime.UtcNow,
                Role = role
            };

            await _repo.CreateAsync(user);
            return user;
        }

        public async Task<string?> LoginAsync(string email, string password)
        {
            var user = await _repo.GetByEmailAsync(email);
            if (user == null) return null;
            if (!VerifyPassword(password, user.Password)) return null;

            // read JWT settings with safe defaults
            var jwtKey = _config["Jwt:Key"] ?? "default_secret_key_please_change";
            var jwtIssuer = _config["Jwt:Issuer"] ?? "taskify";
            var jwtAudience = _config["Jwt:Audience"] ?? "taskify_users";

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[] {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.UserEmail),
                new Claim(ClaimTypes.Role, string.IsNullOrEmpty(user.Role) ? "User" : user.Role)
            };

            var token = new JwtSecurityToken(
                issuer: jwtIssuer,
                audience: jwtAudience,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(6),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private static string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(bytes);
        }

        private static bool VerifyPassword(string password, string hashed)
        {
            return HashPassword(password) == hashed;
        }
    }
}
