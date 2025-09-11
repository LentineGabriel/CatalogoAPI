using AutoMapper;
using CatagoloAPI.Context;
using CatagoloAPI.DTO;
using CatagoloAPI.Models;
using CatagoloAPI.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CatagoloAPI.Controllers;

[Route("[controller]")]
[ApiController]
public class CategoriasController : ControllerBase
{
    private readonly IUnitOfWork _uof;
    private readonly ILogger<CategoriasController> _logger;
    private readonly IMapper _mapper;

    public CategoriasController(IUnitOfWork uof, ILogger<CategoriasController> logger, IMapper mapper)
    {
        _uof = uof;
        _logger = logger;
        _mapper = mapper;
    }

    // GET
    [HttpGet]
    public ActionResult<IEnumerable<CategoriaDTO>> Get()
    {
        _logger.LogInformation("===== Get/TodasAsCategorias =====");
        var categorias = _uof.CategoriaRepository.GetAll();

        if (categorias == null)
        {
            _logger.LogInformation("===== Erro 404: Categorias não encontradas =====");
            return NotFound("Categorias não encontradas");
        }

        var categoriasDTO = _mapper.Map<IEnumerable<CategoriaDTO>>(categorias);

        return Ok(categoriasDTO);
    }

    // GET ID
    [HttpGet("{id:int:min(1)}", Name = "ObterCategoria")]
    public ActionResult<CategoriaDTO> Get(int id)
    {
        _logger.LogInformation($"===== Get/Categorias/id = {id} =====");
        var categoriaId = _uof.CategoriaRepository.Get(c => c.CategoriaId == id);

        if (categoriaId == null)
        {
            _logger.LogInformation($"===== Get/Categorias/id = {id} não encontrado =====");
            return NotFound($"Categoria com ID = {id} não encontrado!");
        }

        var categoriaDTO = _mapper.Map<CategoriaDTO>(categoriaId);

        return Ok(categoriaDTO);
    }

    // POST
    [HttpPost("AdicionarCategoria")]
    public ActionResult<CategoriaDTO> Post(CategoriaDTO c)
    {
        _logger.LogInformation("===== Post/Categorias/AdicionarCategoria =====");
        if (c == null)
        {
            _logger.LogInformation("===== Erro 400: Não foi possível adicionar uma nova categoria =====");
            return BadRequest("Não foi possível adicionar uma nova categoria. Tente novamente mais tarde!");
        }

        var categoria = _mapper.Map<Categoria>(c);

        var categoriaCriada = _uof.CategoriaRepository.Create(categoria);
        _uof.Commit();

        var categoriaDTO = _mapper.Map<CategoriaDTO>(categoriaCriada);

        return new CreatedAtRouteResult("ObterCategoria" , new { id = categoriaDTO.CategoriaId } , categoriaDTO);
    }

    // PUT
    [HttpPut("AtualizarCategoria/{id:int:min(1)}")]
    public ActionResult<CategoriaDTO> Put(int id, CategoriaDTO c)
    {
        _logger.LogInformation($"===== Put/Categorias/AtualizarCategoria/id = {id} =====");
        if (id != c.CategoriaId)
        {
            _logger.LogInformation("===== Erro 400: O id é diferente do que consta no banco de dados =====");
            return BadRequest("O id é diferente do que consta no banco de dados");
        }

        var categoria = _mapper.Map<Categoria>(c);

        var categoriaExistente = _uof.CategoriaRepository.Update(categoria);
        _uof.Commit();

        var categoriaDTO = _mapper.Map<CategoriaDTO>(categoriaExistente);

        return Ok(categoriaDTO);
    }

    // DELETE
    [HttpDelete("DeletarCategoria/{id:int:min(1)}")]
    public ActionResult<CategoriaDTO> Delete(int id)
    {
        _logger.LogInformation($"===== Delete/Categorias/AtualizarCategoria/id = {id} =====");
        var deletarCategoria = _uof.CategoriaRepository.Get(c => c.CategoriaId == id);
        
        if (deletarCategoria == null)
        {
            _logger.LogInformation($"===== Categoria com o id = {id} não encontrada =====");
            return NotFound("Categoria não localizada! Verifique o ID digitado");
        }
        
        var categoriaExcluida = _uof.CategoriaRepository.Delete(deletarCategoria);
        _uof.Commit();

        var categoriaExcluidaDTO = _mapper.Map<CategoriaDTO>(categoriaExcluida);
        
        return Ok(categoriaExcluidaDTO);
    }
}
