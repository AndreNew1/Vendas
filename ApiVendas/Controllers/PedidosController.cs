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
        public async Task<IActionResult> Post([FromBody] PedidoView pedido)
        {
            var cadastro = new PedidoCore(pedido).CadastroPedido();
            return cadastro.Status ? Created($"https://localhost/api/pedidos/{cadastro.Resultado.Id}", cadastro.Resultado) : BadRequest( cadastro.Resultado);
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
        public async Task<IActionResult> Get()
        {
            var Lista = new PedidoCore().Lista().Resultado;
            return Lista.Count != 0 ? Ok(Lista) : NotFound("Não existem registros");
        }
        //Busca por Datas Via QUERY
        [HttpGet("por-data")]
        public async Task<IActionResult> GetPorData([FromQuery] string DataInicial, [FromQuery] string DataFinal)
        {
            var retorno = new PedidoCore().PorData(DataInicial, DataFinal);
            return retorno.Status ? Ok(retorno.Resultado) : BadRequest(retorno.Resultado);
        }

        //Busca por Paginação dos elementos pelos parametros passados pela URL
        [HttpGet("{Ordenacao}/{NumPagina}/{TamanhoPagina}")]
        public async Task<IActionResult> BuscaPorPagina(string Ordenacao, int NumPagina, int TamanhoPagina)
        {
            var retorno = new PedidoCore().PorPagina(NumPagina, Ordenacao, TamanhoPagina);
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