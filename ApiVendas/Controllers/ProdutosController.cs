using System.Threading.Tasks;
using Core;
using Microsoft.AspNetCore.Mvc;
using Model;

namespace ApiVendas.Controllers
{
    [Route("api/[controller]")]
    public class ProdutosController : Controller
    {
        //Cadastra um Produto na base de dados
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Produto produto)
        {
            var cadastro = new ProdutoCore(produto).CadastroProduto();
            return cadastro.Status ? Created($"https://localhost/api/produtos/{produto.Id}", cadastro.Resultado) : BadRequest(cadastro);
        }

        //Busca por um produto especifico pelo seu id
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            var produto = new ProdutoCore().ID(id);
            return produto.Status ? Ok(produto.Resultado) : NotFound(produto.Resultado);
        }
        //Retorna todos os produtos existentes
        [HttpGet]
        public async Task<IActionResult> Get() => Ok(new ProdutoCore().Lista().Resultado);

        //Atualização de um produto
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(string id,[FromBody] Produto produto)
        {
            var cadastro = new ProdutoCore(produto).AtualizaProduto(id);
            return cadastro.Status ? Accepted($"https://localhost/api/produtos/{produto.Id}", cadastro.Resultado) : BadRequest(cadastro);
        }

        //Busca por Datas Via QUERY
        [HttpGet("por-data")]
        public async Task<IActionResult> GetPorData([FromQuery] string Date, [FromQuery] string Time)
        {
            var retorno = new ProdutoCore().PorData(Date, Time);
            return retorno.Status ? Ok(retorno.Resultado) : BadRequest(retorno.Resultado);
        }

        //Busca por Paginação dos elementos pelos parametros passados pela URL
        [HttpGet("{direcao}/{Npagina}/{TPagina}")]
        public async Task<IActionResult> BuscaPorPagina(string Direcao, int NPagina, int TPagina)
        {
            var retorno = new ProdutoCore().PorPagina(NPagina, Direcao, TPagina);
            if (retorno.Resultado.Count == 0)
                return BadRequest(retorno.Resultado);

            return retorno.Status ? Ok(retorno.Resultado) : BadRequest(retorno.Resultado);
        }

        //Deleta um produto pelo seu Id
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var cadastro = new ProdutoCore().DeletaProduto(id);
            return cadastro.Status ? NoContent() : NotFound(cadastro.Resultado);
        }
    }
}
