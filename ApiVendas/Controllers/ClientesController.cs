using System.Threading.Tasks;
using Core;
using Microsoft.AspNetCore.Mvc;
using Model;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ApiVendas.Controllers
{
    [Route("api/[controller]")]
    public class ClientesController : Controller
    {
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Cliente cliente)
        {
            var cadastro = new ClienteCore(cliente).CadastroCliente();
            return cadastro.Status ? (IActionResult)Created($"https://localhost/api/clientes/{cliente.Id}", cadastro.Resultado) : (IActionResult)BadRequest(cadastro.Resultado);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            var cliente = new ClienteCore().ID(id);
            return cliente.Status ? (IActionResult)Ok(cliente.Resultado) : (IActionResult)BadRequest(cliente.Resultado);
        }
        [HttpGet]
        public async Task<IActionResult> Get() => Ok(new ClienteCore().Lista().Resultado);

        [HttpGet("por-data")]
        public async Task<IActionResult> GetPorData([FromQuery] string Date, [FromQuery] string Time)
        {
            var retorno = new ClienteCore().PorData(Date, Time);
            return retorno.Status ? Ok(retorno.Resultado) : BadRequest(retorno.Resultado);
        }

        [HttpGet("{direcao}/{Npagina}/{TPagina}")]
        public async Task<IActionResult> BuscaPorPagina(string Direcao, int NPagina, int TPagina)
        {
            var retorno = new ClienteCore().PorPagina(NPagina, Direcao, TPagina);
            return retorno.Status ? Ok(retorno.Resultado) : BadRequest(retorno.Resultado);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(string id,[FromBody] Cliente cliente)
        {
            var cadastro = new ClienteCore(cliente).AtualizaCliente(id);

            return cadastro.Status
                ? (IActionResult)Accepted($"https://localhost/api/clientes/{cliente.Id}", cadastro.Resultado)
                : (IActionResult)BadRequest(cadastro.Resultado);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var cadastro = new ClienteCore().DeletaCliente(id);

            return cadastro.Status ? NoContent() : (IActionResult)NotFound(cadastro.Resultado);
        }
    }
}
