using AutoMapper;
using Core.Util;
using FluentValidation;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Core
{
    public class ProdutoCore : AbstractValidator<Produto>
    {
        private Sistema Db { get; set; }
        private Produto RProduto { get; set; }
        private IMapper Mapper { get; set; }

        public ProdutoCore(IMapper mapper)
        {
            Db = file.ManipulacaoDeArquivos(null).sistema;

            Db = Db ?? new Sistema();

            //Intanciando o mapper
            Mapper = mapper;
        }

        public ProdutoCore(ProdutoView produto, IMapper mapper)
        {
            Db = file.ManipulacaoDeArquivos(null).sistema;

            Db = Db ?? new Sistema();

            //Intanciando o mapper
            Mapper = mapper;

            RProduto = Mapper.Map<Produto>(produto);

            RuleFor(e => e.Nome)
                .MinimumLength(3)
                .NotNull()
                .WithMessage("Nome Invalido");

            RuleFor(e => e.Valor)
                .NotNull()
                .GreaterThanOrEqualTo(1)
                .WithMessage("Valor Invalido");

            RuleFor(e => e.Quantidade)
                .NotEmpty()
                .GreaterThan(1)
                .WithMessage("Quantidade não pode ser menor ou igual a um");
        }

        /// <summary>
        /// Cadastro de um produto
        /// </summary>
        /// <returns>O produto cadastrado</returns>

        public Retorno CadastroProduto()
        {
            var results = Validate(RProduto);

            // Se o modelo é inválido retorno false com a lista de erros
            if (!results.IsValid)
                return new Retorno { Status = false, Resultado = results.Errors.Select(c => c.ErrorMessage) };

            //Se produto ja existir retorno false com mensagem
            if (Db.Produtos.SingleOrDefault(c => c.Nome.ToLower() == RProduto.Nome.ToLower()) != null)
                return new Retorno { Status = false, Resultado = "Produto ja cadastrado" };

            Db.Produtos.Add(RProduto);

            file.ManipulacaoDeArquivos(Db);

            return new Retorno { Status = true, Resultado = Mapper.Map<ProdutoViewRetorno>(RProduto) };
        }

        /// <summary>
        /// Retorna todos os elementos da lista Produto
        /// </summary>
        /// <returns></returns>

        public Retorno Lista()
        {
            var resposta = Mapper.Map<List<ProdutoViewRetorno>>(Db.Produtos.OrderBy(x => x.Nome).ToList());

            return resposta.Count == 0 ?
            new Retorno { Status = false, Resultado = "Não existe dados" } :
            new Retorno { Status = true, Resultado = resposta };
        }

        /// <summary>
        /// Encontra um produto pelo parametro passado
        /// </summary>
        /// <param name="id">parametro para busca do produto</param>
        /// <returns>O Cliente do id passado</returns>

        public Retorno ID(string id)
        {
            try
            {   
                return  new Retorno { Status = true, Resultado = Mapper.Map<ProdutoViewRetorno>(Db.Produtos.Single(x => x.Id == Guid.Parse(id))) };
            }
            catch (Exception) { }
            return new Retorno { Status = false, Resultado = "Produto não existe" };
        }

        /// <summary>
        /// Atualiza um produto no banco de dados
        /// </summary>
        /// <param name="id">parametro usado para achar o produto da atualização</param>
        /// <returns></returns>

        public Retorno AtualizaProduto(string id)
        {
            try
            {
                var produto = Db.Produtos.Single(c => c.Id == Guid.Parse(id));

                Db.Produtos.Remove(produto);
                
                //Troca valores
                Mapper.Map(RProduto, produto);

                if(Db.Produtos.SingleOrDefault(temp=>temp.Nome==produto.Nome)!=null)
                    return new Retorno { Status = false, Resultado = "Produto ja existe no estoque" };

                file.ManipulacaoDeArquivos(Db);

                return new Retorno { Status = true, Resultado = Mapper.Map<ProdutoViewRetorno>(produto) };
            }
            catch (Exception){}
            return new Retorno { Status = false, Resultado = "Produto não existe" };
        }

        /// <summary>
        /// Deleta um produto do base de dados
        /// </summary>
        /// <param name="id">parametro usado para achar o produto da atualização</param>
        /// <returns></returns>

        public Retorno DeletaProduto(string id)
        {
            try {
                RProduto = Db.Produtos.Single(c => c.Id == Guid.Parse(id));

                Db.Produtos.Remove(RProduto);

                file.ManipulacaoDeArquivos(Db);

                return new Retorno { Status = true, Resultado = "Produto Apagado" };
            } catch (Exception){}
            return new Retorno { Status = false, Resultado = "Produto não existe" };
        }     

        /// <summary>
        /// Retorna uma lista de produtos 
        /// </summary>
        /// <param name="DataInicial">Parametro para refinar a busca por uma data inicial</param>
        /// <param name="DataFinal">Parametro para refinar a busca por uma data Final</param>
        /// <returns></returns>

        public Retorno PorData(string DataInicial, string DataFinal)
        {

            if (!DateTime.TryParse(DataInicial, out DateTime date) && !DateTime.TryParse(DataFinal, out DateTime time))
                return new Retorno { Status = false, Resultado = "Dados Invalidos" };

            //Caso Data final seja nula ou errada
            if (!DateTime.TryParse(DataFinal, out time))
                return new Retorno { Status = true, Resultado = Db.Produtos.Where(x => x.DataCadastro >= date) };

            //Caso Data inicial seja nula ou errada
            return !DateTime.TryParse(DataInicial, out date) ?
                 new Retorno { Status = true, Resultado = Mapper.Map<List<ProdutoViewRetorno>>(Db.Produtos.Where(x => x.DataCadastro <= time)) } :
                 new Retorno { Status = true, Resultado = Mapper.Map<List<ProdutoViewRetorno>>(Db.Produtos.Where(x => x.DataCadastro >= date && x.DataCadastro <= time)) };

        }

        /// <summary>
        /// Consulta os Produto 
        /// </summary>
        /// <param name="NumeroPagina">É para controlar a pagina que esta sendo mostrada</param>
        /// <param name="Ordem">É para definir se estara por nome ou data</param>
        /// <param name="TamanhoPagina">Tamanho da pagina</param>
        /// <returns>Retorna uma lista de Produto</returns>

        public Retorno PorPagina(int NumeroPagina, string Ordem, int TamanhoPagina)
        {
            //limite de elementos por paginas
            if (TamanhoPagina > 30) TamanhoPagina = 30;

            try
            {
                var Pagina = Paginas(TamanhoPagina);

                if (Ordem.ToLower() == "Preco" && NumeroPagina >= 1)
                    return new Retorno { Status = true, Resultado = (Mapper.Map<List<Produto>, List<ProdutoViewRetorno>>(Db.Produtos.OrderBy(x => x.Valor).Skip((NumeroPagina - 1) * TamanhoPagina).Take(TamanhoPagina).ToList()), $"Total de paginas: {Pagina}") };

                return Ordem.ToLower() == "Nome" && NumeroPagina >= 1 ?
                      new Retorno { Status = true, Resultado = (Mapper.Map<List<Produto>, List<ProdutoViewRetorno>>(Db.Produtos.OrderBy(x => x.Nome).Skip((NumeroPagina - 1) * TamanhoPagina).Take(TamanhoPagina).ToList()), $"Total de paginas: {Pagina}") } :
                      new Retorno { Status = true, Resultado = (Mapper.Map<List<Produto>, List<ProdutoViewRetorno>>(Db.Produtos.Take(TamanhoPagina).ToList()), $"Total de paginas: {Pagina}") };
            }
            catch (Exception) { }

            //se paginação é não é possivel
            return new Retorno { Status = true, Resultado = (Mapper.Map<List<Produto>, List<ProdutoViewRetorno>>(Db.Produtos.OrderBy(x => x.DataCadastro).Take(15).ToList()), $"Total de paginas: {Paginas(15)}") };
        }

        /// <summary>
        /// Realiza o calculo para saber quantas paginas existem
        /// </summary>
        /// <param name="TamanhoPagina">Divisão das paginas</param>
        /// <returns></returns>
        
        public int Paginas(int TamanhoPagina)
        {
            //Calculo para paginas
            var lista = Db.Clientes.Count;
            var Paginas = lista / TamanhoPagina;
            return lista % TamanhoPagina != 0 ? Paginas += 1 : Paginas;
        }
    }
}
