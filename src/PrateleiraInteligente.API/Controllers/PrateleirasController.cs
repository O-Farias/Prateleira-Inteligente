using Microsoft.AspNetCore.Mvc;
using PrateleiraInteligente.Domain.Entities;
using PrateleiraInteligente.Domain.Interfaces;

namespace PrateleiraInteligente.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PrateleirasController : ControllerBase
    {
        private readonly IPrateleiraService _prateleiraService;

        public PrateleirasController(IPrateleiraService prateleiraService)
        {
            _prateleiraService = prateleiraService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Prateleira>>> GetPrateleiras()
        {
            var prateleiras = await _prateleiraService.GetAllAsync();
            return Ok(prateleiras);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Prateleira>> GetPrateleira(int id)
        {
            var prateleira = await _prateleiraService.GetByIdAsync(id);

            if (prateleira == null)
                return NotFound();

            return Ok(prateleira);
        }

        [HttpPost]
        public async Task<ActionResult<Prateleira>> CreatePrateleira(Prateleira prateleira)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var novaPrateleira = await _prateleiraService.CreateAsync(prateleira);
            return CreatedAtAction(nameof(GetPrateleira), new { id = novaPrateleira.Id }, novaPrateleira);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePrateleira(int id, Prateleira prateleira)
        {
            if (id != prateleira.Id)
                return BadRequest();

            try
            {
                await _prateleiraService.UpdateAsync(prateleira);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePrateleira(int id)
        {
            try
            {
                await _prateleiraService.DeleteAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpGet("{id}/espaco-disponivel")]
        public async Task<ActionResult<bool>> VerificarEspacoDisponivel(int id)
        {
            try
            {
                var temEspaco = await _prateleiraService.TemEspacoDisponivel(id);
                return Ok(temEspaco);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }
    }
}