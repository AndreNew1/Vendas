using System.Threading.Tasks;
using AutoMapper;
using Core;
using Microsoft.AspNetCore.Mvc;
using Model;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ApiVendas.Controllers
{
    [Route("api/[controller]")]
    public class ClientesController : Controller
    {
        public IMapper Mapper { get; set; }

        public ClientesController(IMapper mapper)
        {
            Mapper = mapper;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] ClienteView cliente)
        {
            var cadastro = new ClienteCore(cliente,Mapper).CadastroCliente();
            return cadastro.Status ? Created($"https://localhost/api/clientes/{cadastro.Resultado.Id}", cadastro.Resultado) : BadRequest(cadastro.Resultado);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            var cliente = new ClienteCore(Mapper).ID(id);
            return cliente.Status ? Ok(cliente.Resultado) : BadRequest(cliente.Resultado);
        }
        
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var lista = new ClienteCore(Mapper).Lista();
            return lista.Status ? Ok(lista.Resultado) : BadRequest(lista.Resultado);
        }


        [HttpGet("por-data")]
        public async Task<IActionResult> GetPorData([FromQuery] string DataInicial, [FromQuery] string DataFinal)
        {
            var retorno = new ClienteCore(Mapper).PorData(DataInicial, DataFinal);
            return retorno.Status ? Ok(retorno.Resultado) : BadRequest(retorno.Resultado);
        }

        [HttpGet("paginas")]
        public async Task<IActionResult> BuscaPorPagina([FromQuery] string Ordem ,[FromQuery] int NumeroPagina ,[FromQuery] int TamanhoPagina)
        {
            var retorno = new ClienteCore(Mapper).PorPagina(NumeroPagina, Ordem, TamanhoPagina);

            return retorno.Status ? Ok(retorno.Resultado) : BadRequest(retorno.Resultado);
        }
        
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(string id,[FromBody] ClienteView cliente)
        {
            var cadastro = new ClienteCore(cliente,Mapper).AtualizaCliente(id);

            return cadastro.Status
                ? Accepted($"https://localhost/api/clientes/{cadastro.Resultado.Id}", cadastro.Resultado)
                : BadRequest(cadastro.Resultado);
        }
        
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var cadastro = new ClienteCore(Mapper).DeletaCliente(id);

            return cadastro.Status ? NoContent() : NotFound(cadastro.Resultado);
        }
    }
}
