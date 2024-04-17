using System.ComponentModel.DataAnnotations;

namespace WebApplication2.Models.DTOs
{
    public class UserToAddDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = "";

        [Required]
        public string Password { get; set; } = "";

        [RegularExpression(@"234-[0-9]{10}")]
        public string? PhoneNumber { get; set; }
    }
}
