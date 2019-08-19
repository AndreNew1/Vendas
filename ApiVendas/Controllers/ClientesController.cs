using System;
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

            if (cadastro.Status)
                return Created($"https://localhost/api/produtos/{cliente.Id}", cadastro.Resultado);

            return BadRequest(cadastro.Resultado);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var cliente = new ClienteCore().ID(id);

            if (cliente.Status)
                return Ok(cliente.Resultado);

            return BadRequest(cliente.Resultado);
        }
        [HttpGet]
        public async Task<IActionResult> Get() => Ok(new ClienteCore().Lista().Resultado);

        [HttpPut]
        public async Task<IActionResult> Put([FromBody] Cliente cliente)
        {
            var cadastro = new ClienteCore(cliente).AtualizaCliente();

            if (cadastro.Status)
                return Accepted($"https://localhost/api/produtos/{cliente.Id}", cadastro.Resultado);

            return BadRequest(cadastro.Resultado);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var cadastro = new ClienteCore().DeletaCliente(id);

            if (cadastro.Status)
                return NoContent();

            return NotFound(cadastro.Resultado);
        }
    }
}
