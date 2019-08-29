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
        //Cadastra um Pedido na base de dados
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Pedido pedido)
        {
            var cadastro = new PedidoCore(pedido).CadastroPedido();
            return cadastro.Status ? Created($"https://localhost/api/pedidos/{pedido.Id}", cadastro.Resultado) : BadRequest(cadastro);
        }

        //Busca um por Pedido especifico pelo seu id
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            var produto = new PedidoCore().ID(id);
            return produto.Status ? Ok(produto.Resultado) : NotFound(produto.Resultado);
        }

        //Retorna todos os Pedidos existentes
        [HttpGet]
        public async Task<IActionResult> Get() => Ok(new PedidoCore().Lista().Resultado);

        //Busca por Datas Via QUERY
        [HttpGet("por-data")]
        public async Task<IActionResult> GetPorData([FromQuery] string Date, [FromQuery] string Time)
        {
            var retorno = new PedidoCore().PorData(Date, Time);
            return retorno.Status ? Ok(retorno.Resultado) : BadRequest(retorno.Resultado);
        }

        //Busca por Paginação dos elementos pelos parametros passados pela URL
        [HttpGet("{direcao}/{Npagina}/{TPagina}")]
        public async Task<IActionResult> BuscaPorPagina(string Direcao, int NPagina, int TPagina)
        {
            var retorno = new PedidoCore().PorPagina(NPagina, Direcao, TPagina);
            if (retorno.Resultado.Count == 0)
                return BadRequest(retorno.Resultado);
            return retorno.Status ? Ok(retorno.Resultado) : BadRequest(retorno.Resultado);
        }

        //Deleta um produto pelo seu Id
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var cadastro = new PedidoCore().DeletaPedido(id);
            return cadastro.Status ? NoContent() : NotFound(cadastro.Resultado);
        }
    }
}