using AutoMapper;
using Core.Util;
using FluentValidation;
using Model;
using System;
using System.Linq;

namespace Core
{
    public class ProdutoCore: AbstractValidator<Produto>
    {
        private Sistema Db { get; set; }
        private Produto RProduto { get; set; }
        private IMapper Mapper { get; set; }

        public ProdutoCore()
        {
            Db = file.ManipulacaoDeArquivos(null).sistema;

            if (Db == null)
                Db = new Sistema();

            //Intanciando o mapper
            Mapper = new NewMapper().Config.CreateMapper();
        }

        public ProdutoCore(ProdutoView produto)
        {
            Db = file.ManipulacaoDeArquivos(null).sistema;

            if (Db == null)
                Db = new Sistema();

            //Intanciando o mapper
            Mapper = new NewMapper().Config.CreateMapper();

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
                return new Retorno { Status = false, Resultado = results.Errors.Select(c=>c.ErrorMessage) };

            //Se produto ja existir retorno false com mensagem
            if (Db.Produtos.SingleOrDefault(c => c.Nome.ToLower() == RProduto.Nome.ToLower()) != null)
                return new Retorno { Status = false, Resultado = "Produto ja cadastrado" };

            Db.Produtos.Add(RProduto);
            
            file.ManipulacaoDeArquivos(Db);

            return new Retorno { Status = true, Resultado = RProduto };
        }
        //Retorna uma lista de produtos ordena por nome do produto
        public Retorno Lista()=> new Retorno { Status = true, Resultado =Mapper.Map<ProdutoViewRetorno>(Db.Produtos.OrderBy(x=>x.Nome))};
        /// <summary>
        /// Retorna uma lista de produtos 
        /// </summary>
        /// <param name="DataInicial">Parametro para refinar a busca por uma data inicial</param>
        /// <param name="DataFinal">Parametro para refinar a busca por uma data Final</param>
        /// <returns></returns>
        public Retorno PorData(string DataInicial, string DataFinal)
        {

            if (DateTime.TryParse(DataInicial, out DateTime date) && DateTime.TryParse(DataFinal, out DateTime time))
                return new Retorno { Status = false, Resultado = "Dados Invalidos" };

            //Caso Data final seja nula ou errada
            if (DateTime.TryParse(DataFinal, out  time))
                return new Retorno { Status = true, Resultado = Db.Produtos.Where(x => x.DataCadastro >= date) };

            //Caso Data inicial seja nula ou errada
            return DateTime.TryParse(DataInicial, out date)?
                 new Retorno { Status = true, Resultado = Db.Produtos.Where(x => x.DataCadastro <= time) }:
                 new Retorno { Status = true, Resultado = Db.Produtos.Where(x => x.DataCadastro >= date && x.DataCadastro <= time )};

        }
        /// <summary>
        /// Consulta os clientes 
        /// </summary>
        /// <param name="NumPagina">É para controlar a pagina que esta sendo mostrada</param>
        /// <param name="Ordenacao">É para definir se estara por nome ou data</param>
        /// <param name="TamanhoPagina">Tamanho da pagina</param>
        /// <returns>Retorna uma lista de clientes</returns>
        public Retorno PorPagina(int NumPagina, string Ordenacao, int TamanhoPagina)
        {
            //limite de elementos por paginas
            if (TamanhoPagina > 30) TamanhoPagina = 30;

            //Calculo para paginas
            var lista = Db.Pedidos.Count;
            var Paginas = lista / 15;
            if (lista % 15 != 0) Paginas += 1;

            try
            {
                Paginas = lista / TamanhoPagina;
                if (lista % TamanhoPagina != 0) Paginas += 1;

                if (Ordenacao.ToLower() == "Preco" && NumPagina >= 1)
                    return new Retorno { Status = true, Resultado = (Db.Produtos.OrderBy(x => x.Valor).Skip((NumPagina - 1) * TamanhoPagina).Take(TamanhoPagina), $"Total de paginas: {Paginas}") };

                return Ordenacao.ToLower() == "Nome" && NumPagina >= 1 ?
                      new Retorno { Status = true, Resultado = (Db.Produtos.OrderBy(x => x.Nome).Skip((NumPagina - 1) * TamanhoPagina).Take(TamanhoPagina), $"Total de paginas: {Paginas}") } :
                      new Retorno { Status = true, Resultado = (Db.Produtos.Skip((NumPagina - 1) * TamanhoPagina).Take(TamanhoPagina),$"Total de paginas: {Paginas}") };
            }
            catch (Exception) { }

            //se paginação é não é possivel
            return new Retorno { Status = true, Resultado = (Db.Pedidos.OrderBy(x => x.DataCadastro).Take(15),$"Total de paginas: {Paginas}") };
        }
        /// <summary>
        /// Encontra um produto pelo parametro passado
        /// </summary>
        /// <param name="id">parametro para busca do produto</param>
        /// <returns>O Cliente do id passado</returns>
        public Retorno ID(string id)
        {
            var produto = Db.Produtos.SingleOrDefault(x => x.Id == Guid.Parse(id));

            //Caso produto não existir retorna false com a messagem
            return produto == null
                ? new Retorno { Status = false, Resultado = "Produto não existe" }
                : new Retorno { Status = true, Resultado = produto };
        }
        /// <summary>
        /// Atualiza um produto no banco de dados
        /// </summary>
        /// <param name="id">parametro usado para achar o produto da atualização</param>
        /// <returns></returns>
        public Retorno AtualizaProduto(string id)
        {

            var produto = Db.Produtos.SingleOrDefault(c => c.Id == Guid.Parse(id));

            //Caso produto não existir retorna false com a messagem
            if (produto == null)
                return new Retorno { Status = false, Resultado = "Produto não existe" };

            //Troca valores
            Mapper.Map(RProduto, produto);

            file.ManipulacaoDeArquivos(Db);

            return new Retorno { Status = true, Resultado = Mapper.Map<ProdutoViewRetorno>(produto) };
        }
        /// <summary>
        /// Deleta um produto do base de dados
        /// </summary>
        /// <param name="id">parametro usado para achar o produto da atualização</param>
        /// <returns></returns>
        public Retorno DeletaProduto(string id)
        {
            RProduto = Db.Produtos.SingleOrDefault(c => c.Id == Guid.Parse(id));

            //Caso produto não existir retorna false com a messagem
            if (RProduto == null)
                return new Retorno { Status = false, Resultado = "Produto não existe" };

            Db.Produtos.Remove(RProduto);

            file.ManipulacaoDeArquivos(Db);

            return new Retorno { Status = true, Resultado = "Produto Apagado"};
        }
    }
}
