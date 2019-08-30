using Core.Util;
using FluentValidation;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Core
{
    public class ClienteCore: AbstractValidator<Cliente>
    {
        private Sistema Db { get; set; }
        private Cliente RCliente { get; set; }
        public ClienteCore()
        {
            Db = file.ManipulacaoDeArquivos(null).sistema;

            if (Db == null)
                Db = new Sistema();
        }
        public ClienteCore(Cliente cliente)
        {
            Db = file.ManipulacaoDeArquivos(null).sistema;

            if (Db == null)
                Db = new Sistema();

            RCliente = cliente;

            RuleFor(e => e.Nome)
                .NotNull()
                .MinimumLength(3)
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
                return new Retorno { Status = false, Resultado = results.Errors.Select(c=>c.ErrorMessage)};
            //Se o cliente ja existe retorno false com a mensagem
            if (Db.Clientes.Find(c => c.Nome == RCliente.Nome) != null)
                return new Retorno() { Status = false, Resultado = "Cliente ja cadastrado" };

            Db.Clientes.Add(RCliente);

            file.ManipulacaoDeArquivos(Db);

            return new Retorno() { Status = true, Resultado = RCliente };
        }
        public Retorno Lista()=> new Retorno() { Status = true, Resultado = Db.Clientes.OrderBy(x=>x.Nome) };

        //Busca por id
        public Retorno ID(string id)
        {
            var cliente = Db.Clientes.SingleOrDefault(x => x.Id.ToString() == id);

            // Se o cliente é inválido retorno false com a messagem
            return cliente == null
                ? new Retorno() { Status = false, Resultado = "Cliente não existe" }
                : new Retorno() { Status = true, Resultado = cliente };
        }

        public Retorno PorData(string date, string time)
        {
            //Testa se os dados são datas
            if (!DateTime.TryParse(date, out DateTime date1) && !DateTime.TryParse(time, out DateTime time1))
                return new Retorno() { Status = false, Resultado = "Dados Invalidos" };

            //Caso Data final seja nula ou errada
            if (!DateTime.TryParse(time, out time1))
                return new Retorno() { Status = true, Resultado = Db.Clientes.Where(x => x.DataCadastro >= date1) };

            //Caso Data inicial seja nula ou errada
            return !DateTime.TryParse(date, out date1)
                ? new Retorno() { Status = true, Resultado = Db.Clientes.Where(x => x.DataCadastro <= time1) }
                : new Retorno() { Status = true, Resultado = Db.Clientes.Where(x => x.DataCadastro >= date1 && x.DataCadastro <= time1) };
        }

        public Retorno PorPagina(int NPagina, string Direcao, int TPagina)
        {

            if (Direcao.ToLower() == "asc" && NPagina >= 1 && TPagina >= 1)
                return new Retorno() { Status = true, Resultado = Db.Clientes.OrderBy(x => x.Nome).Skip((NPagina - 1) * TPagina).Take(TPagina).ToList() };

            if (Direcao.ToLower() == "des" && NPagina >= 1 && TPagina >= 1)
                return new Retorno() { Status = true, Resultado = Db.Clientes.OrderByDescending(x => x.Nome).Skip((NPagina - 1) * TPagina).Take(TPagina).ToList() };
            
            //se paginação é não é possivel
            return new Retorno() { Status = false, Resultado = new List<string>() { "Digite as propriedades corretas" } };
        }

        public Retorno AtualizaCliente(string id)
        {

            var cliente = Db.Clientes.Find(c => c.Id.ToString() == id);
            //Se o produto ja existe retorno false com a mensagem
            if (cliente == null)
                return new Retorno { Status = false, Resultado = "Produto não existe" };

            //Se CPF diferente do origina troca
            //O mesmo para Nome
            if (RCliente.CPF != null&&RCliente.CPF.Length==11) cliente.CPF = RCliente.CPF;
            if (RCliente.Nome != null&&RCliente.Nome.Length>=3) cliente.Nome = RCliente.Nome;

            file.ManipulacaoDeArquivos(Db);

            return new Retorno() { Status = true, Resultado = cliente};
        }

        public Retorno DeletaCliente(string id)
        {
            RCliente = Db.Clientes.Find(c => c.Id.ToString() == id);

            if (RCliente == null)
                return new Retorno() { Status = false, Resultado = "Produto não existe" };

            Db.Clientes.Remove(RCliente);
            file.ManipulacaoDeArquivos(Db);

            return new Retorno() { Status = true, Resultado = "Produto Apagado" };
        }
    }
}
