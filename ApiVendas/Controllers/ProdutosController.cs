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

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] ProdutoView produto)
        {
            var cadastro = new ProdutoCore(produto,Mapper).CadastroProduto();
            return cadastro.Status ? Created($"https://localhost/api/produtos/{cadastro.Resultado.Id}",cadastro.Resultado) : BadRequest(cadastro.Resultado);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            var produto = new ProdutoCore(Mapper).ID(id);
            return produto.Status ? Ok(produto.Resultado) : NotFound(produto.Resultado);
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var lista = new ProdutoCore(Mapper).Lista();
            return lista.Status ? Ok(lista.Resultado) : BadRequest(lista.Resultado);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(string id,[FromBody] ProdutoView produto)
        {
            var cadastro = new ProdutoCore(produto,Mapper).AtualizaProduto(id);
            return cadastro.Status ? Accepted($"https://localhost/api/produtos/{cadastro.Resultado.Id}", cadastro.Resultado) : BadRequest(cadastro.Resultado);
        }

        [HttpGet("por-data")]
        public async Task<IActionResult> GetPorData([FromQuery] string DataInicial, [FromQuery] string DataFinal)
        {
            var retorno = new ProdutoCore(Mapper).PorData(DataInicial, DataFinal);
            return retorno.Status ? Ok(retorno.Resultado) : BadRequest(retorno.Resultado);
        }

        [HttpGet("paginas")]
        public async Task<IActionResult> BuscaPorPagina([FromQuery] string Ordem, [FromQuery] int NumeroPagina, [FromQuery] int TamanhoPagina)
        { 
            var retorno = new ProdutoCore(Mapper).PorPagina(NumeroPagina, Ordem, TamanhoPagina);

            return retorno.Status ? Ok(retorno.Resultado) : BadRequest(retorno.Resultado);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var cadastro = new ProdutoCore(Mapper).DeletaProduto(id);
            return cadastro.Status ? NoContent() : NotFound(await cadastro.Resultado);
        }
    }
}
