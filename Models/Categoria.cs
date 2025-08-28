using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CatagoloAPI.Validations;

namespace CatagoloAPI.Models;

[Table("Categorias")]
public class Categoria : IValidatableObject
{
    [Key]
    public int CategoriaId { get; set; }

    [Required(ErrorMessage = "O nome é obrigatório!")]
    [StringLength(80 , MinimumLength = 2 , ErrorMessage = "O nome deve ter entre 2 e 80 caracteres.")]
    public string? CategoriaNome { get; set; }

    [Required]
    [StringLength(300 , ErrorMessage = "A descrição deve ter, no máximo, {1} caracteres.")]
    public string? CategoriaImagemUrl { get; set; }

    /* ===================== */

    public ICollection<Produto>? Produtos { get; set; }

    public Categoria()
    {
        Produtos = new Collection<Produto>();
    }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (!string.IsNullOrEmpty(CategoriaNome))
        {
            var primeiraLetra = CategoriaNome[0].ToString();
            if (primeiraLetra != primeiraLetra.ToUpper())
            {
                yield return new ValidationResult("A primeira letra do nome da categoria deve ser maiúscula.", new []{nameof(CategoriaNome)});
            }
        }
    }
}
