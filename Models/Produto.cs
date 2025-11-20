using CatagoloAPI.Validations;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace CatagoloAPI.Models;

[Table("Produtos")]
public class Produto : IValidatableObject
{
    #region Props
    [Key]
    public int ProdutoId { get; set; }

    [Required(ErrorMessage = "O nome é obrigatório!")]
    [StringLength(80 , MinimumLength = 2 , ErrorMessage = "O nome deve ter entre 2 e 80 caracteres.")]
    public string? ProdutoNome { get; set; }

    [Required]
    [StringLength(300 , ErrorMessage = "A descrição deve ter, no máximo, {1} caracteres.")]
    public string? ProdutoDescricao { get; set; }

    [Required]
    [Range(1 , 100000 , ErrorMessage = "O preço deve estar entre {1} e {2}.")]
    [Column(TypeName = "decimal(10,2)")]
    public decimal ProdutoPreco { get; set; }

    [Required]
    [StringLength(300 , MinimumLength = 5 , ErrorMessage = "O caminho da imagem deve ter entre 5 e 300 caracteres.")]
    public string? ProdutoImagemUrl { get; set; }

    [Required]
    [Range(1 , 999 , ErrorMessage = "O estoque deve estar entre {1} e {2}.")]
    [Column(TypeName = "decimal(10,2)")]
    public float ProdutoEstoque { get; set; }

    [Required]
    public DateTime DataCadastro { get; set; }

    [RequiredCategoriaId]
    public int CategoriaId { get; set; }

    [JsonIgnore]
    public Categoria? Categoria { get; set; }
    #endregion

    #region Methods
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (!string.IsNullOrEmpty(ProdutoNome))
        {
            var primeiraLetra = ProdutoNome[0].ToString();
            if (primeiraLetra != primeiraLetra.ToUpper())
            {
                yield return new ValidationResult("A primeira letra do nome do produto deve ser maiúscula.", new []{nameof(ProdutoNome)});
            }
            
            if (ProdutoEstoque <= 0)
            {
                yield return new ValidationResult("O estoque deve ser maior que zero.", new []{nameof(ProdutoEstoque)});
            }
        }
    }
    #endregion
}
