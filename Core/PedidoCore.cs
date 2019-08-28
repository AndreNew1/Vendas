using Core.Util;
using FluentValidation;
using Model;
using System;
using System.Linq;

namespace Core
{
    public class PedidoCore:AbstractValidator<Pedido>
    {

        private Sistema Db { get; set; }
        private Pedido RPedido { get; set; }
        public PedidoCore()
        {
            //Ler arquivo
            Db = file.ManipulacaoDeArquivos(null).sistema;
            //Caso arquivo não existe 
            if (Db == null)
                Db = new Sistema();
        }
        public PedidoCore(Pedido pedido)
        {
            //Ler arquivo
            Db = file.ManipulacaoDeArquivos(null).sistema;
            //Caso arquivo não existe 
            if (Db == null)
                Db = new Sistema();

            RPedido = pedido;

            RuleFor(e => e._cliente)
            .NotNull()
            .NotEmpty()
            .Must(ValidaCliente)
            .WithMessage("Cliente não existe na base de dados");

            RuleFor(e => e.Compras)
            .NotNull()
            .NotEmpty()
            .ForEach(e => e.Must(ValidaLista).WithMessage("O produto nâo existe na base de dados"));

        }

        public Retorno CadastroPedido()
        {

            var results = Validate(RPedido);

            // Se o modelo é inválido retorno false
            if (!results.IsValid)
                return new Retorno { Status = false, Resultado = results.Errors.Select(c=>c.ErrorMessage) };

            //Se o pedido ja existe retorno false com a mensagem
            if (Db.Pedidos.SingleOrDefault(c => c.Id == RPedido.Id) != null)
                return new Retorno() { Status = false, Resultado = "Pedido ja cadastrado" };

            //Calculo valor total
            RPedido.ValorTotalAPagar();

            Db.Pedidos.Add(RPedido);
            //Reescreve arquivo
            file.ManipulacaoDeArquivos(Db);

            return new Retorno() { Status = true, Resultado = RPedido };
        }
        public Retorno Lista() => new Retorno() { Status = true, Resultado = Db.Pedidos.OrderBy(x=>x._cliente.Nome)};

        public Retorno PorData(string date, string time)
        {
            //Testa se os dados são datas
            if (!DateTime.TryParse(date, out DateTime date1) && !DateTime.TryParse(time, out DateTime time1))
                return new Retorno() { Status = false, Resultado = "Dados Invalidos" };

            //Caso Data final seja nula ou errada
            if (!DateTime.TryParse(time, out time1))
                return new Retorno() { Status = true, Resultado = Db.Pedidos.Where(x => x.DataCadastro >= date1) };

            //Caso Data inicial seja nula ou errada
            if (!DateTime.TryParse(date, out date1))
                return new Retorno() { Status = true, Resultado = Db.Pedidos.Where(x => x.DataCadastro <= time1) };

            return new Retorno() { Status = true, Resultado = Db.Pedidos.Where(x => x.DataCadastro >= date1 && x.DataCadastro <= time1) };
        }

        public Retorno ID(string id)
        {
            var pedido = Db.Pedidos.SingleOrDefault(x => x.Id.ToString() == id);

            //Se o pedido Não exista retorno false com a mensagem
            return pedido == null
                ? new Retorno() { Status = false, Resultado = "Pedido não existe" }
                : new Retorno() { Status = true, Resultado = pedido };
        }

        public Retorno PorPagina(int NPagina, string Direcao, int TPagina)
        {

            if (Direcao.ToLower() == "asc" && NPagina >= 1 && TPagina >= 1)
                return new Retorno() { Status = true, Resultado = Db.Pedidos.OrderBy(x => x.DataCadastro).Skip((NPagina - 1) * TPagina).Take(TPagina).ToList() };

            if (Direcao.ToLower() == "des" && NPagina >= 1 && TPagina >= 1)
                return new Retorno() { Status = true, Resultado = Db.Pedidos.OrderByDescending(x => x.DataCadastro).Skip((NPagina - 1) * TPagina).Take(TPagina).ToList() };

            //se paginação é não é possivel
            return new Retorno() { Status = false, Resultado = "Digite as propriedades corretas" };
        }

        public Retorno AtualizaPedido(string id)
        {
            var pedido = Db.Pedidos.Find(c => c.Id.ToString() == id);

            //Se o produto Não existe retorno false com a mensagem
            if (pedido == null)
                return new Retorno { Status = false, Resultado = "Pedido não existe" };

            //Define valor Total do pedido
            RPedido.ValorTotalAPagar();
            //Reescreve arquivo
            file.ManipulacaoDeArquivos(Db);

            return new Retorno() { Status = true, Resultado = pedido };
        }

        public Retorno DeletaPedido(string id)
        {
            RPedido = Db.Pedidos.SingleOrDefault(c => c.Id.ToString() == id);
            //Se o produto Não existe retorno false com a mensagem
            if (RPedido == null)
                return new Retorno() { Status = false, Resultado = "Pedido não existe" };
 
            Db.Pedidos.Remove(RPedido);

            //Reescreve arquivo 
            file.ManipulacaoDeArquivos(Db);

            return new Retorno() { Status = true, Resultado = "Pedido Apagado" };
        }
        //Validação de cliente em um pedido
        public bool ValidaCliente(Cliente cliente)
        {
            var tempcliente = Db.Clientes.SingleOrDefault(e => e.Id == cliente.Id);
            if (tempcliente == null) return false;
            if (tempcliente.Nome != cliente.Nome) return false;
            if (tempcliente.CPF != cliente.CPF) return false;
            return true;
        }
        //Valida a lista de produtos
        public bool ValidaLista(Produto produto)
        {
            var tempProd = Db.Produtos.SingleOrDefault(e => e.Id == produto.Id);
            if (tempProd==null) return false;
            if (tempProd.Nome != produto.Nome) return false;
            if (tempProd.Valor != produto.Valor) return false;
            return true;
        }
    }
}
