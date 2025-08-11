using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Tribe.Data
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
        public DateTime LastLoginAt { get; set; }
        public bool IsActive { get; set; } = true;

    }


    // Models/LoginResponse.cs
   
    public class LoginResponse
    {
        public string Token { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }

}
