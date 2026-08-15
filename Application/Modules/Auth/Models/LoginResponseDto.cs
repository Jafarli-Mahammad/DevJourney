using System;

namespace Application.Modules.Auth.Models
{
    public class LoginResponseDto
    {
        public string AccessToken { get; set; } = null!;
        public DateTime ExpiresAt { get; set; }
        public UserDto User { get; set; } = null!;
    }

    public class UserDto
    {
        public Guid Id { get; set; }
        public string Email { get; set; } = null!;
        public string FullName { get; set; } = null!;
        public string Role { get; set; } = null!;
        public Guid? UniversityId { get; set; }
        public string? AvatarUrl { get; set; }
        public bool IsVerified { get; set; }
    }
}
