using Microsoft.AspNetCore.Mvc;
using PrateleiraInteligente.Domain.Entities;
using PrateleiraInteligente.Infrastructure.Persistence;

[Route("api/[controller]")]
[ApiController]
public class AlimentosController : ControllerBase
{
    private readonly AppDbContext _context;

    public AlimentosController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public IActionResult GetAlimentos()
    {
        var alimentos = _context.Alimentos.ToList();
        return Ok(alimentos);
    }

    [HttpPost]
    public IActionResult AddAlimento([FromBody] Alimento alimento)
    {
        _context.Alimentos.Add(alimento);
        _context.SaveChanges();
        return CreatedAtAction(nameof(GetAlimentos), new { id = alimento.Id }, alimento);
    }
}
