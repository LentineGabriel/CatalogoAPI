using System.ComponentModel.DataAnnotations;

namespace CatagoloAPI.DTOs;
public class LoginModelDTO
{
    [Required(ErrorMessage = "User Name is required")]
    public string? Username { get; set; }

    [Required(ErrorMessage = "Password is required")]
    public string? Password { get; set; }
}
