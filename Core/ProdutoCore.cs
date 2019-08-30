using Core.Util;
using FluentValidation;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Core
{
    public class ProdutoCore: AbstractValidator<Produto>
    {
        private Sistema Db { get; set; }
        private Produto RProduto { get; set; }
        public ProdutoCore()
        {
            Db= file.ManipulacaoDeArquivos(null).sistema;

            if (Db == null)
                Db = new Sistema();
        }

        public ProdutoCore(Produto produto)
        {
            Db = file.ManipulacaoDeArquivos(null).sistema;

            if (Db == null)
                Db = new Sistema();

            RProduto = produto;

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

        public Retorno CadastroProduto()
        {
            var results = Validate(RProduto);

            // Se o modelo é inválido retorno false
            if (!results.IsValid)
                return new Retorno { Status = false, Resultado = results.Errors.Select(c=>c.ErrorMessage) };

            //Se produto ja existir retorno false com menssagem
            if (Db.Produtos.Find(c => c.Nome == RProduto.Nome) != null)
                return new Retorno() { Status = false, Resultado = "Produto ja cadastrado" };

            Db.Produtos.Add(RProduto);
            
            file.ManipulacaoDeArquivos(Db);

            return new Retorno() { Status = true, Resultado = RProduto };
        }
        //Retorna lista ordena por nome do produto
        public Retorno Lista()=> new Retorno() { Status = true, Resultado = Db.Produtos.OrderBy(x=>x.Nome)};

        public Retorno PorData(string date,string time)
        {
            //Testa se os dados são datas
            if (DateTime.TryParse(date, out DateTime date1) && DateTime.TryParse(time, out DateTime time1))
                return new Retorno() { Status = true, Resultado = Db.Produtos.Where(x => x.DataCadastro >= date1 && x.DataCadastro <= time1) };

            //Caso Data final seja nula ou errada
            if (!DateTime.TryParse(time, out time1))
                return new Retorno() { Status = true, Resultado = Db.Produtos.Where(x => x.DataCadastro >= date1) };

            //Caso Data inicial seja nula ou errada
            return !DateTime.TryParse(date, out date1)
                ? new Retorno() { Status = true, Resultado = Db.Produtos.Where(x => x.DataCadastro <= time1) }
                : new Retorno() { Status = false, Resultado = "Dados Invalidos" };
        }

        public Retorno PorPagina(int NPagina, string Direcao, int TPagina)
        {

            if (Direcao.ToLower() == "asc" && NPagina >= 1 && TPagina >= 1)
                return new Retorno() { Status = true, Resultado = Db.Produtos.OrderBy(x => x.Nome).Skip((NPagina - 1) * TPagina).Take(TPagina).ToList() };

            if (Direcao.ToLower() == "des" && NPagina >= 1 && TPagina >= 1)
                return new Retorno() { Status = true, Resultado = Db.Produtos.OrderByDescending(x => x.Nome).Skip((NPagina - 1) * TPagina).Take(TPagina).ToList() };

            //se paginação é não é possivel
            return new Retorno() { Status = false, Resultado = new List<string>() { "Digite as propriedades corretas" } };
        }

        public Retorno ID(string id)
        {
            var produto = Db.Produtos.SingleOrDefault(x => x.Id.ToString() == id);

            //Caso produto não existir retorna false com a messagem
            return produto == null
                ? new Retorno() { Status = false, Resultado = "Produto não existe" }
                : new Retorno() { Status = true, Resultado = produto };
        }
        public Retorno AtualizaProduto(string id)
        {

            var produto = Db.Produtos.Find(c => c.Id.ToString() == id);

            //Caso produto não existir retorna false com a messagem
            if (produto == null)
                return new Retorno { Status = false, Resultado = "Produto não existe" };

            //Troca valores
            if (RProduto.Valor != 0) produto.Valor = RProduto.Valor;
            if (RProduto.Nome != null) produto.Nome = RProduto.Nome;
            if (RProduto.Quantidade != produto.Quantidade) produto.Quantidade = RProduto.Quantidade;

            file.ManipulacaoDeArquivos(Db);

            return new Retorno() { Status = true, Resultado = produto };
        }

        public Retorno DeletaProduto(string id)
        {
            RProduto = Db.Produtos.Find(c => c.Id.ToString() == id);

            //Caso produto não existir retorna false com a messagem
            if (RProduto == null)
                return new Retorno() { Status = false, Resultado = "Produto não existe" };

            Db.Produtos.Remove(RProduto);

            file.ManipulacaoDeArquivos(Db);

            return new Retorno() { Status = true, Resultado = "Produto Apagado"};
        }
    }
}
