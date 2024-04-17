using Microsoft.AspNetCore.Identity;

namespace WebApplication2.Models.Entities
{
    public class AppUser: IdentityUser
    {
        public string? PublicId { get; set; }
        public string? PhotoUrl { get; set; }
    }
}
