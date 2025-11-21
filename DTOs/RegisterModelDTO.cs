using System.ComponentModel.DataAnnotations;

namespace CatagoloAPI.DTOs;
public class RegisterModelDTO
{
    [Required(ErrorMessage = "User Name is required")]
    public string? Username { get; set; }

    [EmailAddress]
    [Required(ErrorMessage = "User Name is required")]
    public string? Email { get; set; }

    [Required(ErrorMessage = "User Name is required")]
    public string? Password { get; set; }
}
