using CatagoloAPI.Context;
using CatagoloAPI.Models;
using CatagoloAPI.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CatagoloAPI.Controllers;

[Route("[controller]")]
[ApiController]
public class CategoriasController : ControllerBase
{
    private readonly ICategoriaRepository _repo;
    private readonly ILogger<CategoriasController> _logger;

    public CategoriasController(ICategoriaRepository repo, ILogger<CategoriasController> logger)
    {
        _repo = repo;
        _logger = logger;
    }

    // GET
    [HttpGet]
    public ActionResult<IEnumerable<Categoria>> Get()
    {
        _logger.LogInformation("===== Get/TodasAsCategorias =====");
        var categorias = _repo.GetAll();

        if (categorias == null)
        {
            _logger.LogInformation("===== Erro 404: Categorias não encontradas =====");
            return NotFound("Categorias não encontradas");
        }

        return Ok(categorias);
    }

    // GET COM PRODUTOS
    [HttpGet("ComProdutos")]
    public ActionResult<IEnumerable<Categoria>> GetCategoriasProdutos()
    {
        _logger.LogInformation("===== Get/Categorias/ComProdutos =====");
        var produtos = _repo.GetAllWithProducts();
        
        return Ok(produtos);
    }

    // GET ID
    [HttpGet("{id:int:min(1)}", Name = "ObterCategoria")]
    public ActionResult<Categoria> Get(int id)
    {
        _logger.LogInformation($"===== Get/Categorias/id = {id} =====");
        var categoria = _repo.GetById(id);

        if (categoria == null)
        {
            _logger.LogInformation($"===== Get/Categorias/id = {id} não encontrado =====");
            return NotFound($"Categoria com ID = {id} não encontrado!");
        }

        return Ok(categoria);
    }

    // POST
    [HttpPost("AdicionarCategoria")]
    public ActionResult<Categoria> Post(Categoria c)
    {
        _logger.LogInformation("===== Post/Categorias/AdicionarCategoria =====");
        if (c == null)
        {
            _logger.LogInformation("===== Erro 400: Não foi possível adicionar uma nova categoria =====");
            return BadRequest("Não foi possível adicionar uma nova categoria. Tente novamente mais tarde!");
        }

        var categoriaCriada = _repo.Create(c);

        return new CreatedAtRouteResult("ObterCategoria" , new { id = categoriaCriada.CategoriaId } , categoriaCriada);
    }

    // PUT
    [HttpPut("AtualizarCategoria/{id:int:min(1)}")]
    public ActionResult<Categoria> Put(int id, Categoria c)
    {
        _logger.LogInformation($"===== Put/Categorias/AtualizarCategoria/id = {id} =====");
        if (id != c.CategoriaId)
        {
            _logger.LogInformation("===== Erro 400: O id é diferente do que consta no banco de dados =====");
            return BadRequest("O id é diferente do que consta no banco de dados");
        }

        var categoriaExistente = _repo.Update(c);

        return Ok(categoriaExistente);
    }

    // DELETE
    [HttpDelete("DeletarCategoria/{id:int:min(1)}")]
    public ActionResult<Categoria> Delete(int id)
    {
        _logger.LogInformation($"===== Delete/Categorias/AtualizarCategoria/id = {id} =====");
        var deletarCategoria = _repo.GetById(id);
        if (deletarCategoria == null)
        {
            _logger.LogInformation($"===== Categoria com o id = {id} não encontrada =====");
            return NotFound("Categoria não localizada! Verifique o ID digitado");
        }
        
        var categoriaExcluida = _repo.Delete(id);
        
        return Ok(categoriaExcluida);
    }
}
