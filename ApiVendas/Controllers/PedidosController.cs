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

        //Cadastra um Pedido na base de dados
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] PedidoView pedido)
        {
            var cadastro = new PedidoCore(pedido,Mapper).CadastroPedido();
            return cadastro.Status ? Created($"https://localhost/api/pedidos/{cadastro.Resultado.Id}", cadastro.Resultado) : BadRequest( cadastro.Resultado);
        }

        //Busca um por Pedido especifico pelo seu id
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            var produto = new PedidoCore(Mapper).ID(id);
            return produto.Status ? Ok(produto.Resultado) : NotFound(produto.Resultado);
        }

        //Retorna todos os Pedidos existentes
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var Lista = new PedidoCore(Mapper).Lista();
            return Lista.Status ? Ok(Lista.Resultado) : BadRequest(Lista.Resultado);
        }
        //Busca por Datas Via QUERY
        [HttpGet("por-data")]
        public async Task<IActionResult> GetPorData([FromQuery] string DataInicial, [FromQuery] string DataFinal)
        {
            var retorno = new PedidoCore(Mapper).PorData(DataInicial, DataFinal);
            return retorno.Status ? Ok(retorno.Resultado) : BadRequest(retorno.Resultado);
        }

        /// <summary>
        /// Busca por Paginação dos elementos pelos parametros passados pela URL
        /// </summary>
        /// <param name="Ordenacao">Ordena a lista pela string passada</param>
        /// <param name="NumPagina">Numero da pagina desejada</param>
        /// <param name="TamanhoPagina"></param>
        /// <returns></returns>
       
        [HttpGet("{Ordenacao}/{NumPagina}/{TamanhoPagina}")]
        public async Task<IActionResult> BuscaPorPagina(string Ordenacao, int NumPagina, int TamanhoPagina)
        {
            var retorno = new PedidoCore(Mapper).PorPagina(NumPagina, Ordenacao, TamanhoPagina);
            return retorno.Status ? Ok(retorno.Resultado) : BadRequest(retorno.Resultado);
        }

        /// <summary>
        /// Deleta um Pedido da base de dados
        /// </summary>
        /// <param name="id">parametro passado para procurar um Pedido</param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var cadastro = new PedidoCore(Mapper).DeletaPedido(id);
            return cadastro.Status ? NoContent() : NotFound(cadastro.Resultado);
        }
    }
}