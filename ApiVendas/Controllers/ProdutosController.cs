using System.Threading.Tasks;
using AutoMapper;
using Core;
using Microsoft.AspNetCore.Mvc;
using Model;

namespace ApiVendas.Controllers
{
    [Route("api/[controller]")]
    public class ProdutosController : Controller
    {
        public IMapper Mapper { get; set; }


        public ProdutosController(IMapper mapper)
        {
            Mapper = mapper;
        }

        //Cadastra um Produto na base de dados
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] ProdutoView produto)
        {
            var cadastro = new ProdutoCore(produto,Mapper).CadastroProduto();
            return cadastro.Status ? Created($"https://localhost/api/produtos/{cadastro.Resultado.Id}",cadastro.Resultado) : BadRequest(cadastro.Resultado);
        }

        //Busca por um produto especifico pelo seu id
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            var produto = new ProdutoCore(Mapper).ID(id);
            return produto.Status ? Ok(produto.Resultado) : NotFound(produto.Resultado);
        }
        //Retorna todos os produtos existentes
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var lista = new ProdutoCore(Mapper).Lista();
            return lista.Status ? Ok(lista.Resultado) : BadRequest(lista.Resultado);
        }
        //Atualização de um produto
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(string id,[FromBody] ProdutoView produto)
        {
            var cadastro = new ProdutoCore(produto,Mapper).AtualizaProduto(id);
            return cadastro.Status ? Accepted($"https://localhost/api/produtos/{cadastro.Resultado.Id}", cadastro.Resultado) : BadRequest(cadastro.Resultado);
        }

        //Busca por uma lista de produtos baseado em suas datas
        [HttpGet("por-data")]
        public async Task<IActionResult> GetPorData([FromQuery] string DataInicial, [FromQuery] string DataFinal)
        {
            var retorno = new ProdutoCore(Mapper).PorData(DataInicial, DataFinal);
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
            var retorno = new ProdutoCore(Mapper).PorPagina(NumPagina, Ordenacao, TamanhoPagina);

            return retorno.Status ? Ok(retorno.Resultado) : BadRequest(retorno.Resultado);
        }

        /// <summary>
        /// Deleta um produto da base de dados
        /// </summary>
        /// <param name="id">parametro passado para procurar um produto</param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var cadastro = new ProdutoCore(Mapper).DeletaProduto(id);
            return cadastro.Status ? NoContent() : NotFound(await cadastro.Resultado);
        }
    }
}
