using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CatagoloAPI.DTO;

public class CategoriaDTO
{
    public int CategoriaId { get; set; }

    [Required(ErrorMessage = "O nome é obrigatório!")]
    [StringLength(80 , MinimumLength = 2 , ErrorMessage = "O nome deve ter entre 2 e 80 caracteres.")]
    public string? CategoriaNome { get; set; }

    [Required]
    [StringLength(300 , ErrorMessage = "A descrição deve ter, no máximo, {1} caracteres.")]
    public string? CategoriaImagemUrl { get; set; }
}