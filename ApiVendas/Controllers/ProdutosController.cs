﻿using System.Threading.Tasks;
using Core;
using Microsoft.AspNetCore.Mvc;
using Model;

namespace ApiVendas.Controllers
{
    [Route("api/[controller]")]
    public class ProdutosController : Controller
    {
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Produto produto)
        {
            var cadastro = new ProdutoCore(produto).CadastroProduto();
            return cadastro.Status ? (IActionResult)Created($"https://localhost/api/produtos/{produto.Id}", cadastro.Resultado) : BadRequest(cadastro);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            var produto = new ProdutoCore().ID(id);
            return produto.Status ? (IActionResult)Ok(produto.Resultado) : (IActionResult)NotFound(produto.Resultado);
        }
        [HttpGet]
        public async Task<IActionResult> Get() => Ok(new ProdutoCore().Lista().Resultado);

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(string id,[FromBody] Produto produto)
        {
            var cadastro = new ProdutoCore(produto).AtualizaProduto(id);
            return cadastro.Status ? (IActionResult)Accepted($"https://localhost/api/produtos/{produto.Id}", cadastro.Resultado) : BadRequest(cadastro);
        }

        [HttpGet("por-data")]
        public async Task<IActionResult> GetPorData([FromQuery] string Date, [FromQuery] string Time)
        {
            var retorno = new ProdutoCore().PorData(Date, Time);
            return retorno.Status ? Ok(retorno.Resultado) : BadRequest(retorno.Resultado);
        }


        [HttpGet("{direcao}/{Npagina}/{TPagina}")]
        public async Task<IActionResult> BuscaPorPagina(string Direcao, int NPagina, int TPagina)
        {
            var retorno = new ProdutoCore().PorPagina(NPagina, Direcao, TPagina);
            return retorno.Status ? Ok(retorno.Resultado) : BadRequest(retorno.Resultado);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var cadastro = new ProdutoCore().DeletaProduto(id);
            return cadastro.Status ? NoContent() : (IActionResult)NotFound(cadastro.Resultado);
        }
    }
}
