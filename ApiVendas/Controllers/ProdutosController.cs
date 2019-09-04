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
        public async Task<IActionResult> Post([FromBody] ProdutoView produto)
        {
            var cadastro = new ProdutoCore(produto).CadastroProduto();
            return cadastro.Status ? Created($"https://localhost/api/produtos/{cadastro.Resultado.Id}",cadastro.Resultado) : BadRequest(cadastro.Resultado);
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
        public async Task<IActionResult> Get()
        {
            var lista = new ProdutoCore().Lista().Resultado;
            return lista.count != 0 ? Ok(lista) : NotFound("Não existe base de dados");
        }
        //Atualização de um produto
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(string id,[FromBody] ProdutoView produto)
        {
            var cadastro = new ProdutoCore(produto).AtualizaProduto(id);
            return cadastro.Status ? Accepted($"https://localhost/api/produtos/{cadastro.Resultado.Id}", cadastro.Resultado) : BadRequest(cadastro.Resultado);
        }

        //Busca por uma lista de produtos baseado em suas datas
        [HttpGet("por-data")]
        public async Task<IActionResult> GetPorData([FromQuery] string DataInicial, [FromQuery] string DataFinal)
        {
            var retorno = new ProdutoCore().PorData(DataInicial, DataFinal);
            return retorno.Status ? Ok(retorno.Resultado) : BadRequest(retorno.Resultado);
        }

        //Busca por Paginação dos elementos pelos parametros passados pela URL
        [HttpGet("{Ordenacao}/{NumPagina}/{TamanhoPagina}")]
        public async Task<IActionResult> BuscaPorPagina(string Ordenacao, int NumPagina, int TamanhoPagina)
        { 
            var retorno = new ProdutoCore().PorPagina(NumPagina, Ordenacao, TamanhoPagina);

            return retorno.Status ? Ok(retorno.Resultado) : BadRequest(retorno.Resultado);
        }

        //Deleta um produto pelo seu Id
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var cadastro = new ProdutoCore().DeletaProduto(id);
            return cadastro.Status ? NoContent() : NotFound(await cadastro.Resultado);
        }
    }
}
