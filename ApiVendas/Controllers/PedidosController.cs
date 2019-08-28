using System.Threading.Tasks;
using Core;
using Microsoft.AspNetCore.Mvc;
using Model;

namespace ApiVendas.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PedidosController : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Pedido pedido)
        {
            var cadastro = new PedidoCore(pedido).CadastroPedido();
            return cadastro.Status ? (IActionResult)Created($"https://localhost/api/pedidos/{pedido.Id}", cadastro.Resultado) : BadRequest(cadastro);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            var produto = new PedidoCore().ID(id);
            return produto.Status ? (IActionResult)Ok(produto.Resultado) : (IActionResult)NotFound(produto.Resultado);
        }

        [HttpGet]
        public async Task<IActionResult> Get() => Ok(new PedidoCore().Lista().Resultado);

        [HttpGet("por-data")]
        public async Task<IActionResult> GetPorData([FromQuery] string Date, [FromQuery] string Time)
        {
            var retorno = new PedidoCore().PorData(Date, Time);
            return retorno.Status ? Ok(retorno.Resultado) : BadRequest(retorno.Resultado);
        }

        [HttpGet("{direcao}/{Npagina}/{TPagina}")]
        public async Task<IActionResult> BuscaPorPagina(string Direcao, int NPagina, int TPagina)
        {
            var retorno = new PedidoCore().PorPagina(NPagina, Direcao, TPagina);
            return retorno.Status ? Ok(retorno.Resultado) : BadRequest(retorno.Resultado);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(string id,[FromBody] Pedido pedido)
        {
            var cadastro = new PedidoCore(pedido).AtualizaPedido(id);
            return cadastro.Status ? (IActionResult)Accepted($"https://localhost/api/pedidos/{pedido.Id}", cadastro.Resultado) : BadRequest(cadastro);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var cadastro = new PedidoCore().DeletaPedido(id);
            return cadastro.Status ? NoContent() : (IActionResult)NotFound(cadastro.Resultado);
        }
    }
}