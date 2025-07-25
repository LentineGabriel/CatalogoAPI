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
    public ActionResult<IEnumerable<Categoria>> Get()
    {
        try
        {
            var categorias = _context.Categorias.AsNoTracking().ToList();

            if(categorias == null) return NotFound();

            return categorias;
        }
        catch(Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError , "Ocorreu um problema ao tratar a sua solicitação.");
        }
    }

    [HttpGet("produtos")]
    public ActionResult<IEnumerable<Categoria>> GetCategoriasProdutos()
    {
        // return _context.Categorias.Include(p => p.Produtos).AsNoTracking().ToList();
        return _context.Categorias.Include(p => p.Produtos).Where(c => c.CategoriaId <= 5).AsNoTracking().ToList();
    }

    // GET ID
    [HttpGet("{id:int}" , Name = "ObterCategoria")]
    public ActionResult<Categoria> Get(int id)
    {
        var categoria = _context.Categorias.FirstOrDefault(c => c.CategoriaId == id);

        if(categoria == null) return NotFound();

        return Ok(categoria);
    }

    // POST
    [HttpPost]
    public ActionResult Post(Categoria c)
    {
        if(c == null) return BadRequest();

        _context.Categorias.Add(c);
        _context.SaveChanges();

        return new CreatedAtRouteResult("ObterCategoria" , new { id = c.CategoriaId } , c);
    }

    // PUT
    [HttpPut("{id:int}")]
    public ActionResult Put(int id , Categoria c)
    {
        if(id != c.CategoriaId) return BadRequest();

        _context.Entry(c).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
        _context.SaveChanges();

        return Ok(c);
    }

    // DELETE
    [HttpDelete("{id:int}")]
    public ActionResult Delete(int id)
    {
        var categoria = _context.Categorias.FirstOrDefault(c => c.CategoriaId == id);

        if(categoria == null) return NotFound("Categoria não localizada!");

        _context.Categorias.Remove(categoria);
        _context.SaveChanges();

        return Ok(categoria);
    }
}
