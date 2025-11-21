using Microsoft.AspNetCore.Identity;

namespace CatagoloAPI.Models;
public class ApplicationUser : IdentityUser
{
    public string? RefreshToken { get; set; }
    public DateTime RefreshTokenExpiryTime { get; set; }
}
