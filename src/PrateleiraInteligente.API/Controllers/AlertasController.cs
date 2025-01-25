using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PrateleiraInteligente.Domain.Entities;
using PrateleiraInteligente.Domain.Interfaces;

namespace PrateleiraInteligente.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AlertasController : ControllerBase
    {
        private readonly IAlertaService _alertaService;

        public AlertasController(IAlertaService alertaService)
        {
            _alertaService = alertaService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Alerta>>> GetAlertas()
        {
            var alertas = await _alertaService.GetAlertasNaoResolvidosAsync();
            return Ok(alertas);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Alerta>> GetAlerta(int id)
        {
            var alerta = await _alertaService.GetByIdAsync(id);

            if (alerta == null)
                return NotFound();

            return Ok(alerta);
        }

        [HttpPost]
        public async Task<ActionResult<Alerta>> CreateAlerta(Alerta alerta)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var novoAlerta = await _alertaService.CreateAlertaAsync(alerta);
            return CreatedAtAction(nameof(GetAlerta), new { id = novoAlerta.Id }, novoAlerta);
        }

        [HttpPut("{id}/resolver")]
        public async Task<IActionResult> ResolverAlerta(int id)
        {
            try
            {
                await _alertaService.ResolverAlertaAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpPost("verificar-vencimentos")]
        public async Task<IActionResult> VerificarVencimentos()
        {
            await _alertaService.VerificarProdutosVencimentoAsync();
            return Ok();
        }

        [HttpPost("verificar-estoque")]
        public async Task<IActionResult> VerificarEstoque()
        {
            await _alertaService.VerificarEstoqueBaixoAsync();
            return Ok();
        }
    }
}