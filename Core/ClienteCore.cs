using Core.Util;
using FluentValidation;
using Model;
using System;
using System.Linq;

namespace Core
{
    public class ClienteCore: AbstractValidator<Cliente>
    {
        private Sistema Db { get; set; }
        private Cliente RCliente { get; set; }
        public ClienteCore()
        {
            Db = file.ManipulacaoDeArquivos(true, null).sistema;

            if (Db == null)
                Db = new Sistema();
        }
        public ClienteCore(Cliente cliente)
        {
            Db = file.ManipulacaoDeArquivos(true, null).sistema;

            if (Db == null)
                Db = new Sistema();

            RCliente = cliente;
            RuleFor(e => e.Nome)
                .MinimumLength(4)
                .NotNull()
                .WithMessage("Nome Invalido");
            RuleFor(e => e.CPF)
                .NotNull()
                .Length(11,11)
                .WithMessage("Cpf Invalido");

        }

        public Retorno CadastroCliente()
        {

            var results = Validate(RCliente);

            // Se o modelo é inválido retorno false
            if (!results.IsValid)
                return new Retorno { Status = false, Resultado = results.Errors };

            if (Db.Clientes.Find(c => c.Nome == RCliente.Nome) != null)
                return new Retorno() { Status = false, Resultado = "Cliente ja cadastrado" };

            Db.Clientes.Add(RCliente);
            file.ManipulacaoDeArquivos(false, Db);

            return new Retorno() { Status = true, Resultado = RCliente };
        }
        public Retorno Lista()=> new Retorno() { Status = true, Resultado = Db.Clientes };

        public Retorno ID(Guid id)
        {
            var cliente = Db.Clientes.SingleOrDefault(x => x.Id == id);

            if (cliente == null)
                return new Retorno() { Status = false, Resultado = "Cliente não existe" };

            return new Retorno() { Status = true, Resultado = cliente };
        }
        public Retorno AtualizaCliente()
        {

            var cliente = Db.Clientes.Find(c => c.Id == RCliente.Id);
            if (cliente == null)
                return new Retorno { Status = false, Resultado = "Produto não existe" };

            if (RCliente.CPF != null) cliente.CPF = RCliente.CPF;
            if (RCliente.Nome != null) cliente.Nome = RCliente.Nome;

            file.ManipulacaoDeArquivos(false, Db);

            return new Retorno() { Status = true, Resultado = cliente};
        }

        public Retorno DeletaCliente(Guid id)
        {
            RCliente = Db.Clientes.Find(c => c.Id == id);

            if (RCliente == null)
                return new Retorno() { Status = false, Resultado = "Produto não existe" };

            Db.Clientes.Remove(RCliente);
            file.ManipulacaoDeArquivos(false, Db);

            return new Retorno() { Status = true, Resultado = "Produto Apagado" };
        }
    }
}
