using CatagoloAPI.Context;
using CatagoloAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CatagoloAPI.Controllers;

[Route("[controller]")]
[ApiController]
public class CategoriasController : ControllerBase
{
    private readonly AppDbContext _context;

    public CategoriasController(AppDbContext context)
    {
        _context = context;
    }

    // GET
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Categoria>>> Get()
    {
        try
        {
            var categorias = await _context.Categorias.AsNoTracking().ToListAsync();
            if(categorias == null) return NotFound();

            return categorias;
        }
        catch(Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError , "Ocorreu um problema ao tratar a sua solicitação.");
        }
    }

    [HttpGet("ComProdutos")]
    public async Task<ActionResult<IEnumerable<Categoria>>> GetCategoriasProdutos()
    {
        try
        {
            var categorias = await _context.Categorias.Include(p => p.Produtos).AsNoTracking().ToListAsync();

            return categorias;
        }
        catch(Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError , "Ocorreu um problema ao tratar a sua solicitação.");
        }
    }

    // GET ID
    [HttpGet("{id:int:min(1)}" , Name = "ObterCategoria")]
    public async Task<ActionResult<Categoria>> Get(int id)
    {
        try
        {
            var categoria = await _context.Categorias.FirstOrDefaultAsync(c => c.CategoriaId == id);
            if(categoria == null) return NotFound();

            return categoria;
        }
        catch(Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError , "Ocorreu um problema ao tratar a sua solicitação.");
        }
    }

    // POST
    [HttpPost("AdicionarCategoria")]
    public async Task<ActionResult> Post(Categoria c)
    {
        try
        {
            if(c == null) return BadRequest();

            await _context.Categorias.AddAsync(c);
            await _context.SaveChangesAsync();

            return new CreatedAtRouteResult("ObterCategoria" , new { id = c.CategoriaId } , c);
        }
        catch(Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError , "Ocorreu um problema ao tratar a sua solicitação.");
        }
    }

    // PUT
    [HttpPut("AtualizarCategoria/{id:int:min(1)}")]
    public async Task<ActionResult> Put(int id , Categoria c)
    {
        try
        {
            if(id != c.CategoriaId) return BadRequest();

            var categoriaExistente = await _context.Categorias.FindAsync(id);
            if(categoriaExistente == null) return NotFound("Categoria não localizada!");

            categoriaExistente.CategoriaNome = c.CategoriaNome;
            categoriaExistente.CategoriaImagemUrl = c.CategoriaImagemUrl;

            await _context.SaveChangesAsync();
            return Ok(categoriaExistente);
        }
        catch(Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError , "Ocorreu um problema ao tratar a sua solicitação.");
        }
    }

    // DELETE
    [HttpDelete("DeletarCategoria/{id:int:min(1)}")]
    public async Task<ActionResult> Delete(int id)
    {
        var categoria = await _context.Categorias.FirstOrDefaultAsync(c => c.CategoriaId == id);
        if(categoria == null) return NotFound("Categoria não localizada!");

        _context.Categorias.Remove(categoria);
        await _context.SaveChangesAsync();

        return Ok(categoria);
    }
}
