using Microsoft.AspNetCore.Identity;

namespace MyTribe.Data
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
        // JWT Refresh Token Support
        public string? RefreshToken { get; set; }
        public DateTime RefreshTokenExpiry { get; set; }

        // Zusätzliche Sicherheits-Eigenschaften
        public DateTime? LastLoginDate { get; set; }
        public string? LastLoginIp { get; set; }
        public string? LastUserAgent { get; set; }

    }

}
