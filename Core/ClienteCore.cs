using Core.Util;
using FluentValidation;
using Model;
using System;
using System.Linq;
using AutoMapper;
using System.Collections.Generic;

namespace Core
{
    public class ClienteCore : AbstractValidator<Cliente>
    {
        private Sistema Db { get; set; }
        private Cliente RCliente { get; set; }
        private IMapper Mapper { get; set; }

        public ClienteCore(IMapper mapper)
        {
            Db = file.ManipulacaoDeArquivos(null).sistema;

            Db = Db ?? new Sistema();

            Mapper = mapper;
        }

        public ClienteCore(ClienteView cliente,IMapper mapper)
        {
            Db = file.ManipulacaoDeArquivos(null).sistema;

            Db = Db ?? new Sistema();

            Mapper = mapper; 
            
            RCliente = Mapper.Map<ClienteView, Cliente>(cliente);

            RuleFor(e => e.Nome)
                .NotNull()
                .MinimumLength(3)
                .WithMessage("Nome Invalido");

            RuleFor(e => e.CPF)
                .NotNull()
                .Length(11, 11)
                .WithMessage("Cpf Invalido");

        }

        /// <summary>
        /// Cadastro de um cliente 
        /// </summary>
        /// <returns>O cliente que acabou de se cadastrar</returns>
        
        public Retorno CadastroCliente()
        {

            var results = Validate(RCliente);

            // Se o modelo é inválido retorno false
            if (!results.IsValid)
                return new Retorno { Status = false, Resultado = results.Errors.Select(c => c.ErrorMessage) };

            //Se o cliente ja existe retorno false com a mensagem
            if (Db.Clientes.SingleOrDefault(c => c.CPF == RCliente.CPF) != null)
                return new Retorno() { Status = false, Resultado = "Cliente ja cadastrado" };

            Db.Clientes.Add(RCliente);

            file.ManipulacaoDeArquivos(Db);


            return new Retorno() { Status = true, Resultado = Mapper.Map<ClienteViewRetorno>(RCliente) };
        }

        /// <summary>
        /// Retorna todos os elementos da lista Produto
        /// </summary>
        /// <returns></returns>
        
        public Retorno Lista() {

            var resposta = Mapper.Map<List<ClienteViewRetorno>>(Db.Clientes.OrderBy(x => x.Nome).ToList());

                return resposta.Count!=0?
                    new Retorno { Status = true, Resultado = resposta }:
                    new Retorno { Status = false, Resultado = "A consulta falhou" };
        }

        /// <summary>
        /// Busca um cliente baseado em seu Id
        /// </summary>
        /// <param name="id">parametro para comparação para encontra determinado cliente</param>
        /// <returns></returns>
        
        public Retorno ID(string id)
        {
            try
            {
                return new Retorno { Status = true, Resultado = Mapper.Map<ClienteViewRetorno>(Db.Clientes.Single(x => x.Id == Guid.Parse(id))) };
            }
            catch (Exception) { }
            return new Retorno { Status = false, Resultado = "Cliente não existe" };
        }

        /// <summary>
        /// Atualiza as informações de um cliente
        /// </summary>
        /// <param name="id">Id passado indentificar o cliente para alterar suas propriedades</param>
        /// <returns>o Novo cliente</returns>
        
        public Retorno AtualizaCliente(string id)
        {
            try
            {
                var cliente = Db.Clientes.Single(c => c.Id == Guid.Parse(id));

                Db.Clientes.Remove(cliente);

                //Atualiza o cliente
                Mapper.Map(RCliente, cliente);
                
                if(Db.Clientes.SingleOrDefault(temp=>temp.CPF==cliente.CPF)!=null)
                    return new Retorno { Status = false, Resultado = "CPF ja cadastrada na base de dados" };

                file.ManipulacaoDeArquivos(Db);

                return new Retorno { Status = true, Resultado = Mapper.Map<ClienteViewRetorno>(cliente) };
            }
            catch (Exception) { }
            return new Retorno { Status = false, Resultado = "Cliente não existe" };
        }

        /// <summary>
        /// Deleta um cliente com o determinado ID passado
        /// </summary>
        /// <param name="id">ID de um cliente</param>
        /// <returns></returns>
        
        public Retorno DeletaCliente(string id)
        {
            try
            {
                //Consulta o banco de clientes
                RCliente = Db.Clientes.Single(c => c.Id == Guid.Parse(id));

                Db.Clientes.Remove(RCliente);

                file.ManipulacaoDeArquivos(Db);

                return new Retorno { Status = true, Resultado = "Cliente Apagado" };
            }
            catch (Exception) { }

            return new Retorno { Status = false, Resultado = "Cliente não Encontrado" };
        }
         
        /// <summary>
        /// Consulta os clientes 
        /// </summary>
        /// <param name="NumeroPagina">É para controlar a pagina que esta sendo mostrada</param>
        /// <param name="Ordenacao">É para definir se estara de ordem crescente ou descrescente</param>
        /// <param name="TamanhoPagina">Tamanho da pagina</param>
        /// <returns>Retorna uma lista de clientes</returns>
        
        public Retorno PorPagina(int NumeroPagina, string Ordem, int TamanhoPagina)
        {
            //limite de elementos por paginas
            if (TamanhoPagina > 30) TamanhoPagina = 30;

            try
            {
                var Pagina = Paginas(TamanhoPagina);

                if (Ordem.ToLower() == "cpf" && NumeroPagina >= 1)
                    return new Retorno { Status = true, Resultado = (Mapper.Map<List<ClienteViewRetorno>>(Db.Clientes.OrderBy(x => x.CPF).Skip((NumeroPagina - 1) * TamanhoPagina).Take(TamanhoPagina).ToList()), $"Total de paginas: {Pagina}") };

                return Ordem.ToLower() == "Nome" && NumeroPagina >= 1 ?
                      new Retorno { Status = true, Resultado = ( Mapper.Map<List<ClienteViewRetorno>>( Db.Clientes.OrderBy(x => x.Nome).Skip((NumeroPagina - 1) * TamanhoPagina).Take(TamanhoPagina).ToList()), $"Total de paginas: {Pagina}") } :
                      new Retorno { Status = true, Resultado = ( Mapper.Map<List<ClienteViewRetorno>>( Db.Clientes.Skip((NumeroPagina - 1) * TamanhoPagina).Take(TamanhoPagina).ToList()), $"Total de paginas: {Pagina}") };
            }
            catch (Exception) { }
            
            //se paginação é não é possivel
            return new Retorno { Status = true, Resultado = ( Mapper.Map<List<ClienteViewRetorno>>( Db.Clientes.Take(15).ToList()), $"Total de paginas: {Paginas(15)}") };
        }

        /// <summary>
        /// retorna uma lista de clientes de uma determinada data de inicio e fim
        /// </summary>
        /// <param name="DataInicial">É para que a lista seja afinada com uma data Inicial</param>
        /// <param name="DataFinal">É para que a lista seja afinada com uma data maxima</param>
        /// <returns></returns>
        
        public Retorno PorData(string DataInicial, string DataFinal)
        {
            if (!DateTime.TryParse(DataInicial, out DateTime date) && !DateTime.TryParse(DataFinal, out DateTime time))
                return new Retorno { Status = false, Resultado = "Dados Invalidos" };

            //Caso Data final seja nula ou errada
            if (!DateTime.TryParse(DataFinal, out time))
                return new Retorno { Status = true, Resultado = Mapper.Map<List<ClienteViewRetorno>>(Db.Clientes.Where(x => x.DataCadastro >= date).ToList()) };

            //Caso Data inicial seja nula ou errada
            return !DateTime.TryParse(DataInicial, out date)
                ? new Retorno { Status = true, Resultado = Mapper.Map<List<ClienteViewRetorno>>(Db.Clientes.Where(x => x.DataCadastro <= date).ToList()) }
                : new Retorno { Status = true, Resultado = Mapper.Map<List<ClienteViewRetorno>>(Db.Clientes.Where(x => x.DataCadastro >= date && x.DataCadastro <= time).ToList()) };
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
