using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PrateleiraInteligente.Domain.Entities;
using PrateleiraInteligente.Infrastructure.Persistence;

[ApiController]
[Route("api/[controller]")]
public class MovimentacoesController : ControllerBase
{
    private readonly AppDbContext _context;

    public MovimentacoesController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Movimentacao>>> GetMovimentacoes()
    {
        return await _context.Movimentacoes.Include(m => m.Produto).ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Movimentacao>> GetMovimentacao(int id)
    {
        var movimentacao = await _context.Movimentacoes
            .Include(m => m.Produto)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (movimentacao == null)
        {
            return NotFound();
        }

        return movimentacao;
    }

    [HttpPost]
    public async Task<ActionResult<Movimentacao>> CreateMovimentacao(Movimentacao movimentacao)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        _context.Movimentacoes.Add(movimentacao);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetMovimentacao), new { id = movimentacao.Id }, movimentacao);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateMovimentacao(int id, Movimentacao movimentacao)
    {
        if (id != movimentacao.Id)
        {
            return BadRequest();
        }

        _context.Entry(movimentacao).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!MovimentacaoExists(id))
            {
                return NotFound();
            }
            throw;
        }

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteMovimentacao(int id)
    {
        var movimentacao = await _context.Movimentacoes.FindAsync(id);

        if (movimentacao == null)
        {
            return NotFound();
        }

        _context.Movimentacoes.Remove(movimentacao);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool MovimentacaoExists(int id)
    {
        return _context.Movimentacoes.Any(e => e.Id == id);
    }
}