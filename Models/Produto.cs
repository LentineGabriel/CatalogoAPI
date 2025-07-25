using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace CatagoloAPI.Models;

[Table("Produtos")]
public class Produto
{
    [Key]
    public int ProdutoId { get; set; }

    [Required]
    [StringLength(80)]
    public string? ProdutoNome { get; set; }

    [Required]
    [StringLength(300)]
    public string? ProdutoDescricao { get; set; }

    [Required]
    [Column(TypeName = "decimal(10,2)")]
    public decimal ProdutoPreco { get; set; }

    [Required]
    [StringLength(300)]
    public string? ProdutoImagemUrl { get; set; }
    public float ProdutoEstoque { get; set; }
    public DateTime DataCadastro { get; set; }
    public int CategoriaId { get; set; }

    [JsonIgnore]
    public Categoria? Categoria { get; set; }
}
