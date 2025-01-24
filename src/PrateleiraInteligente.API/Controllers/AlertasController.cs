using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PrateleiraInteligente.Domain.Entities;
using PrateleiraInteligente.Infrastructure.Persistence;

namespace PrateleiraInteligente.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AlertasController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AlertasController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Alerta>>> GetAlertas()
        {
            return await _context.Alertas
                .Include(a => a.Produto)
                .ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Alerta>> GetAlerta(int id)
        {
            var alerta = await _context.Alertas
                .Include(a => a.Produto)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (alerta == null)
            {
                return NotFound();
            }

            return alerta;
        }

        [HttpGet("nao-resolvidos")]
        public async Task<ActionResult<IEnumerable<Alerta>>> GetAlertasNaoResolvidos()
        {
            return await _context.Alertas
                .Include(a => a.Produto)
                .Where(a => !a.Resolvido)
                .ToListAsync();
        }

        [HttpPost]
        public async Task<ActionResult<Alerta>> CreateAlerta(Alerta alerta)
        {
            alerta.DataCriacao = DateTime.Now;
            alerta.Resolvido = false;

            _context.Alertas.Add(alerta);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetAlerta), new { id = alerta.Id }, alerta);
        }

        [HttpPut("{id}/resolver")]
        public async Task<IActionResult> ResolverAlerta(int id)
        {
            var alerta = await _context.Alertas.FindAsync(id);

            if (alerta == null)
            {
                return NotFound();
            }

            alerta.Resolvido = true;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAlerta(int id)
        {
            var alerta = await _context.Alertas.FindAsync(id);

            if (alerta == null)
            {
                return NotFound();
            }

            _context.Alertas.Remove(alerta);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}