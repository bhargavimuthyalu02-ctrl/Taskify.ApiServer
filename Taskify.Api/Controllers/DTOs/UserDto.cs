namespace Taskify.Api.Controllers.DTOs
{
	public class RegisterDto
	{
		public string UserEmail { get; set; } = default!;
		public string Password { get; set; } = default!;
		public string? Role { get; set; }
	}

	public class LoginDto
	{
		public string UserEmail { get; set; } = default!;
		public string Password { get; set; } = default!;
	}
}
