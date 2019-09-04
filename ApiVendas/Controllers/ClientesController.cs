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

        /// <summary>
        /// Cadastra um cliente no banco de dados
        /// </summary>
        /// <param name="cliente">modelo de um clienteView</param>
        /// <returns>retorna um objeto cliente</returns>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] ClienteView cliente)
        {
            var cadastro = new ClienteCore(cliente).CadastroCliente();
            return cadastro.Status ? Created($"https://localhost/api/clientes/{cadastro.Resultado.Id}", cadastro.Resultado) : BadRequest(cadastro.Resultado);
        }

        /// <summary>
        /// Busca um cliente na base de dados
        /// </summary>
        /// <param name="id">parametro para procura</param>
        /// <returns>retorna um objeto cliente do id passado</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            var cliente = new ClienteCore().ID(id);
            return cliente.Status ? Ok(cliente.Resultado) : BadRequest(cliente.Resultado);
        }

        //Retorna uma lista com todos os clientes existentes
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var lista = new ClienteCore().Lista().Resultado;
            return lista.Count != 0 ? Ok(lista) : BadRequest("Não existem clientes na base de dados");
        }
        //Busca por Datas Via QUERY
        [HttpGet("por-data")]
        public async Task<IActionResult> GetPorData([FromQuery] string DataInicial, [FromQuery] string DataFinal)
        {
            var retorno = new ClienteCore().PorData(DataInicial, DataFinal);
            return retorno.Status ? Ok(retorno.Resultado) : BadRequest(retorno.Resultado);
        }

        //Busca por Paginação dos elementos pelos parametros passados pela URL
        [HttpGet("{Ordenacao}/{NumPagina}/{TamanhoPagina}")]
        public async Task<IActionResult> BuscaPorPagina(string Ordenacao, int NumPagina, int TamanhoPagina)
        {
            var retorno = new ClienteCore().PorPagina(NumPagina, Ordenacao, TamanhoPagina);

            return retorno.Status ? Ok(retorno.Resultado) : BadRequest(retorno.Resultado);
        }

        /// <summary>
        /// Atualiza um cliente 
        /// </summary>
        /// <param name="id">parametro para achar o cliente para modificação</param>
        /// <param name="cliente">Model para alterar os dados do cliente</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(string id,[FromBody] ClienteView cliente)
        {
            var cadastro = new ClienteCore(cliente).AtualizaCliente(id);

            return cadastro.Status
                ? Accepted($"https://localhost/api/clientes/{cadastro.Resultado.Id}", cadastro.Resultado)
                : BadRequest(cadastro.Resultado);
        }

        /// <summary>
        /// Deleta um produto da base de dados
        /// </summary>
        /// <param name="id">paramet</param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var cadastro = new ClienteCore().DeletaCliente(id);

            return cadastro.Status ? NoContent() : NotFound(cadastro.Resultado);
        }
    }
}
