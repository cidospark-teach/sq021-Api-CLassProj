using System.ComponentModel.DataAnnotations;

namespace WebApplication2.Models.DTOs
{
    public class UserUpdateDto
    {

        [RegularExpression(@"234-[0-9]{10}")]
        public string? PhoneNumber { get; set; }
    }
}
