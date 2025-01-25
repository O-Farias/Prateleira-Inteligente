using Microsoft.AspNetCore.Mvc;
using PrateleiraInteligente.Domain.Entities;
using PrateleiraInteligente.Domain.Interfaces;

namespace PrateleiraInteligente.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProdutosController : ControllerBase
    {
        private readonly IProdutoService _produtoService;

        public ProdutosController(IProdutoService produtoService)
        {
            _produtoService = produtoService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Produto>>> GetProdutos()
        {
            var produtos = await _produtoService.GetAllAsync();
            return Ok(produtos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Produto>> GetProduto(int id)
        {
            try
            {
                var produto = await _produtoService.GetByIdAsync(id);
                return Ok(produto);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpPost]
        public async Task<ActionResult<Produto>> CreateProduto(Produto produto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var novoProduto = await _produtoService.CreateAsync(produto);
                return CreatedAtAction(nameof(GetProduto), new { id = novoProduto.Id }, novoProduto);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduto(int id, Produto produto)
        {
            if (id != produto.Id)
                return BadRequest();

            try
            {
                await _produtoService.UpdateAsync(produto);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduto(int id)
        {
            try
            {
                await _produtoService.DeleteAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpGet("vencimento-proximo/{diasAviso}")]
        public async Task<ActionResult<IEnumerable<Produto>>> GetProdutosProximosVencimento(int diasAviso)
        {
            var produtos = await _produtoService.GetProdutosProximosVencimentoAsync(diasAviso);
            return Ok(produtos);
        }

        [HttpGet("estoque-baixo/{quantidadeMinima}")]
        public async Task<ActionResult<IEnumerable<Produto>>> GetProdutosBaixoEstoque(int quantidadeMinima)
        {
            var produtos = await _produtoService.GetProdutosBaixoEstoqueAsync(quantidadeMinima);
            return Ok(produtos);
        }
    }
}