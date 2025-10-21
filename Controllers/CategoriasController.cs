using AutoMapper;
using CatagoloAPI.Context;
using CatagoloAPI.DTO;
using CatagoloAPI.Models;
using CatagoloAPI.Pagination;
using CatagoloAPI.Pagination.Categorias;
using CatagoloAPI.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

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

    #region GET
    // GET
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CategoriaDTO>>> GetAsync()
    {
        _logger.LogInformation("===== Get/TodasAsCategorias =====");
        var categorias = await _uof.CategoriaRepository.GetAllAsync();

        if (categorias == null)
        {
            _logger.LogInformation("===== Erro 404: Categorias não encontradas =====");
            return NotFound("Categorias não encontradas");
        }

        var categoriasDTO = _mapper.Map<IEnumerable<CategoriaDTO>>(categorias);

        return Ok(categoriasDTO);
    }

    // GET PAGINAÇÃO
    [HttpGet("Paginacao")]
    public async Task<ActionResult<IEnumerable<Categoria>>> GetPaginationAsync([FromQuery] CategoriasParameters categoriasParameters)
    {
        var categorias = await _uof.CategoriaRepository.GetCategoriesAsync(categoriasParameters);

        return ObterCategorias(categorias);
    }

    // GET CATEGORIAS C/ FILTRO POR NOME
    [HttpGet("nome")]
    public async Task<ActionResult<IEnumerable<Categoria>>> GetFilterNomePaginationAsync([FromQuery] CategoriasFiltroNome categoriasFiltroNome)
    {
        var categorias = await _uof.CategoriaRepository.GetCategoriesFilteringByNameAsync(categoriasFiltroNome);
        return ObterCategorias(categorias);
    }

    // GET ID
    [HttpGet("{id:int:min(1)}", Name = "ObterCategoria")]
    public async Task<ActionResult<CategoriaDTO>> GetIdAsync(int id)
    {
        _logger.LogInformation($"===== Get/Categorias/id = {id} =====");
        var categoriaId = await _uof.CategoriaRepository.GetIdAsync(c => c.CategoriaId == id);

        if (categoriaId == null)
        {
            _logger.LogInformation($"===== Get/Categorias/id = {id} não encontrado =====");
            return NotFound($"Categoria com ID = {id} não encontrado!");
        }

        var categoriaDTO = _mapper.Map<CategoriaDTO>(categoriaId);

        return Ok(categoriaDTO);
    }
    #endregion

    #region POST
    // POST
    [HttpPost("AdicionarCategoria")]
    public async Task<ActionResult<CategoriaDTO>> PostAsync(CategoriaDTO c)
    {
        _logger.LogInformation("===== Post/Categorias/AdicionarCategoria =====");
        if (c == null)
        {
            _logger.LogInformation("===== Erro 400: Não foi possível adicionar uma nova categoria =====");
            return BadRequest("Não foi possível adicionar uma nova categoria. Tente novamente mais tarde!");
        }

        var categoria = _mapper.Map<Categoria>(c);

        var categoriaCriada = _uof.CategoriaRepository.Create(categoria);
        await _uof.CommitAsync();

        var categoriaDTO = _mapper.Map<CategoriaDTO>(categoriaCriada);

        return new CreatedAtRouteResult("ObterCategoria" , new { id = categoriaDTO.CategoriaId } , categoriaDTO);
    }
    #endregion

    #region PUT
    // PUT
    [HttpPut("AtualizarCategoria/{id:int:min(1)}")]
    public async Task<ActionResult<CategoriaDTO>> PutAsync(int id, CategoriaDTO c)
    {
        _logger.LogInformation($"===== Put/Categorias/AtualizarCategoria/id = {id} =====");
        if (id != c.CategoriaId)
        {
            _logger.LogInformation("===== Erro 400: O id é diferente do que consta no banco de dados =====");
            return BadRequest("O id é diferente do que consta no banco de dados");
        }

        var categoria = _mapper.Map<Categoria>(c);

        var categoriaExistente = _uof.CategoriaRepository.Update(categoria);
        await _uof.CommitAsync();

        var categoriaDTO = _mapper.Map<CategoriaDTO>(categoriaExistente);

        return Ok(categoriaDTO);
    }
    #endregion

    #region DELETE
    // DELETE
    [HttpDelete("DeletarCategoria/{id:int:min(1)}")]
    public async Task<ActionResult<CategoriaDTO>> DeleteAsync(int id)
    {
        _logger.LogInformation($"===== Delete/Categorias/AtualizarCategoria/id = {id} =====");
        var deletarCategoria = await _uof.CategoriaRepository.GetIdAsync(c => c.CategoriaId == id);
        
        if (deletarCategoria == null)
        {
            _logger.LogInformation($"===== Categoria com o id = {id} não encontrada =====");
            return NotFound("Categoria não localizada! Verifique o ID digitado");
        }
        
        var categoriaExcluida = _uof.CategoriaRepository.Delete(deletarCategoria);
        await _uof.CommitAsync();

        var categoriaExcluidaDTO = _mapper.Map<CategoriaDTO>(categoriaExcluida);
        
        return Ok(categoriaExcluidaDTO);
    }
    #endregion

    // OTHER METHODS
    private ActionResult<IEnumerable<Categoria>> ObterCategorias(PagedList<Categoria> categorias)
    {
        var metadata = new
        {
            categorias.TotalCount ,
            categorias.PageSize ,
            categorias.CurrentPage ,
            categorias.TotalPages ,
            categorias.HasNext ,
            categorias.HasPrevious
        };
        Response.Headers.Append("X-Pagination" , JsonConvert.SerializeObject(metadata));

        var categoriaDTO = _mapper.Map<IEnumerable<CategoriaDTO>>(categorias);
        return Ok(categoriaDTO);
    }
}
