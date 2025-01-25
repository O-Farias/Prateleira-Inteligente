using Microsoft.AspNetCore.Mvc;
using PrateleiraInteligente.Domain.Entities;
using PrateleiraInteligente.Domain.Interfaces;

namespace PrateleiraInteligente.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MovimentacoesController : ControllerBase
    {
        private readonly IMovimentacaoService _movimentacaoService;

        public MovimentacoesController(IMovimentacaoService movimentacaoService)
        {
            _movimentacaoService = movimentacaoService;
        }

        [HttpGet("produto/{produtoId}")]
        public async Task<ActionResult<IEnumerable<Movimentacao>>> GetMovimentacoesPorProduto(int produtoId)
        {
            var movimentacoes = await _movimentacaoService.GetMovimentacoesPorProdutoAsync(produtoId);
            return Ok(movimentacoes);
        }

        [HttpPost("entrada")]
        public async Task<ActionResult<Movimentacao>> RegistrarEntrada(int produtoId, int quantidade, string observacao)
        {
            try
            {
                var movimentacao = await _movimentacaoService.RegistrarEntradaAsync(produtoId, quantidade, observacao);
                return CreatedAtAction(nameof(GetMovimentacoesPorProduto), new { produtoId }, movimentacao);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPost("saida")]
        public async Task<ActionResult<Movimentacao>> RegistrarSaida(int produtoId, int quantidade, string observacao)
        {
            try
            {
                var movimentacao = await _movimentacaoService.RegistrarSaidaAsync(produtoId, quantidade, observacao);
                return CreatedAtAction(nameof(GetMovimentacoesPorProduto), new { produtoId }, movimentacao);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}