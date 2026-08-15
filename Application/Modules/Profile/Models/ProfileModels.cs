using System;

namespace Application.Modules.Profile.Models
{
    public class MeDto
    {
        public Guid Id { get; set; }
        public string Email { get; set; } = null!;
        public string Role { get; set; } = null!;
    }

    public class ProfileDto
    {
        public Guid Id { get; set; }
        public string FullName { get; set; } = null!;
        public string? Bio { get; set; }
        public string? AvatarUrl { get; set; }
    }

    public class PublicProfileDto
    {
        public Guid Id { get; set; }
        public string FullName { get; set; } = null!;
        public string? Bio { get; set; }
        public string? AvatarUrl { get; set; }
    }

    public class CvUploadResultDto
    {
        public Guid AssetId { get; set; }
        public string Url { get; set; } = null!;
    }
}
