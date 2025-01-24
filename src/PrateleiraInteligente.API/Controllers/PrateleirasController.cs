using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PrateleiraInteligente.Domain.Entities;
using PrateleiraInteligente.Infrastructure.Persistence;

[ApiController]
[Route("api/[controller]")]
public class PrateleirasController : ControllerBase
{
    private readonly AppDbContext _context;

    public PrateleirasController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Prateleira>>> GetPrateleiras()
    {
        return await _context.Prateleiras.Include(p => p.Produtos).ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Prateleira>> GetPrateleira(int id)
    {
        var prateleira = await _context.Prateleiras
            .Include(p => p.Produtos)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (prateleira == null)
        {
            return NotFound();
        }

        return Ok(prateleira);
    }

    [HttpPost]
    public async Task<ActionResult<Prateleira>> CreatePrateleira(Prateleira prateleira)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        _context.Prateleiras.Add(prateleira);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetPrateleira), new { id = prateleira.Id }, prateleira);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdatePrateleira(int id, Prateleira prateleira)
    {
        if (id != prateleira.Id)
        {
            return BadRequest();
        }

        _context.Entry(prateleira).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!PrateleiraExists(id))
            {
                return NotFound();
            }
            throw;
        }

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePrateleira(int id)
    {
        var prateleira = await _context.Prateleiras.FindAsync(id);

        if (prateleira == null)
        {
            return NotFound();
        }

        _context.Prateleiras.Remove(prateleira);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool PrateleiraExists(int id)
    {
        return _context.Prateleiras.Any(e => e.Id == id);
    }
}