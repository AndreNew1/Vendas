using System.Threading.Tasks;
using AutoMapper;
using Core;
using Microsoft.AspNetCore.Mvc;
using Model;

namespace ApiVendas.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PedidosController : ControllerBase
    {
        public IMapper Mapper { get; set; }

        public PedidosController(IMapper mapper)
        {
            Mapper = mapper;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] PedidoView pedido)
        {
            var cadastro = new PedidoCore(pedido,Mapper).CadastroPedido();
            return cadastro.Status ? Created($"https://localhost/api/pedidos/{cadastro.Resultado.Id}", cadastro.Resultado) : BadRequest( cadastro.Resultado);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            var produto = new PedidoCore(Mapper).ID(id);
            return produto.Status ? Ok(produto.Resultado) : NotFound(produto.Resultado);
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var Lista = new PedidoCore(Mapper).Lista();
            return Lista.Status ? Ok(Lista.Resultado) : BadRequest(Lista.Resultado);
        }

        [HttpGet("por-data")]
        public async Task<IActionResult> GetPorData([FromQuery] string DataInicial, [FromQuery] string DataFinal)
        {
            var retorno = new PedidoCore(Mapper).PorData(DataInicial, DataFinal);
            return retorno.Status ? Ok(retorno.Resultado) : BadRequest(retorno.Resultado);
        }

        [HttpGet("paginas")]
        public async Task<IActionResult> BuscaPorPagina([FromQuery] string Ordem, [FromQuery] int NumeroPagina, [FromQuery] int TamanhoPagina)
        {
            var retorno = new PedidoCore(Mapper).PorPagina(NumeroPagina, Ordem, TamanhoPagina);
            return retorno.Status ? Ok(retorno.Resultado) : BadRequest(retorno.Resultado);
        }
 
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var cadastro = new PedidoCore(Mapper).DeletaPedido(id);
            return cadastro.Status ? NoContent() : NotFound(cadastro.Resultado);
        }
    }
}