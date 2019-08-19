using System;
using System.Threading.Tasks;
using Core;
using Microsoft.AspNetCore.Mvc;
using Model;

namespace ApiVendas.Controllers
{
    [Route("api/[controller]")]
    public class ProdutosController : Controller
    {
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Produto produto)
        {
            var cadastro = new ProdutoCore(produto).CadastroProduto();
            if (cadastro.Status)
                return Created($"https://localhost/api/produtos/{produto.Id}", cadastro.Resultado);

            return BadRequest(cadastro);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var produto = new ProdutoCore().ID(id);

            if (produto.Status)
                return Ok(produto.Resultado);

            return NotFound(produto.Resultado);
        }
        [HttpGet]
        public async Task<IActionResult> Get() => Ok(new ProdutoCore().Lista().Resultado);

        [HttpPut]
        public async Task<IActionResult> Put([FromBody] Produto produto)
        {
            var cadastro = new ProdutoCore(produto).AtualizaProduto();

            if (cadastro.Status)
                return Accepted($"https://localhost/api/produtos/{produto.Id}", cadastro.Resultado);

            return BadRequest(cadastro);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var cadastro = new ProdutoCore().DeletaProduto(id);

            if (cadastro.Status)
                return NoContent();

            return NotFound(cadastro.Resultado);
        }
    }
}
