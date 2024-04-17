using System.Security.Claims;
using WebApplication2.Models.DTOs;
using WebApplication2.Models.Entities;

namespace WebApplication2.Services.Interfaces
{
    public interface IAuthService
    {
        string GenerateJWT(AppUser user, List<string> roles, List<Claim> claims);

        Task<LoginResult> Login(string email, string password);

        Task<Dictionary<string, string>> ValidateLoggedInUser(ClaimsPrincipal user, string userId);

    }
}
