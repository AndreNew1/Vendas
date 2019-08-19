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
        public ProdutoCore()
        {
            Db= file.ManipulacaoDeArquivos(true, null).sistema;

            if (Db == null)
                Db = new Sistema();
        }
        public ProdutoCore(Produto produto)
        {
            Db = file.ManipulacaoDeArquivos(true, null).sistema;

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
        }

        public Retorno CadastroProduto()
        {
            var results = Validate(RProduto);

            // Se o modelo é inválido retorno false
            if (!results.IsValid)
                return new Retorno { Status = false, Resultado = results.Errors };

            if (Db.Produtos.Find(c => c.Nome == RProduto.Nome) != null)
                return new Retorno() { Status = false, Resultado = "Produto ja cadastrado" };

            Db.Produtos.Add(RProduto);
            file.ManipulacaoDeArquivos(false, Db);

            return new Retorno() { Status = true, Resultado = RProduto };
        }

        public Retorno Lista()=> new Retorno() { Status = true, Resultado = Db.Produtos };


        public Retorno ID(Guid id)
        {
            var produto = Db.Produtos.SingleOrDefault(x => x.Id == id);

            if (produto == null)
                return new Retorno() { Status = false, Resultado = "Produto não existe" };

            return new Retorno() { Status = true, Resultado = produto };
        }
        public Retorno AtualizaProduto()
        {
            var produto = Db.Produtos.Find(c => c.Id == RProduto.Id);

            if (produto == null)
                return new Retorno { Status = false, Resultado = "Produto não existe" };

            if (RProduto.Valor != 0) produto.Valor = RProduto.Valor;
            if (RProduto.Nome != null) produto.Nome = RProduto.Nome;

            file.ManipulacaoDeArquivos(false, Db);

            return new Retorno() { Status = true, Resultado = produto };
        }

        public Retorno DeletaProduto(Guid id)
        {
            RProduto = Db.Produtos.Find(c => c.Id == id);

            if (RProduto == null)
                return new Retorno() { Status = false, Resultado = "Produto não existe" };

            Db.Produtos.Remove(RProduto);
            file.ManipulacaoDeArquivos(false, Db);

            return new Retorno() { Status = true, Resultado = "Produto Apagado"};
        }
    }
}
