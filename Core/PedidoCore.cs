﻿using AutoMapper;
using Core.Util;
using FluentValidation;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Core
{
    public class PedidoCore:AbstractValidator<Pedido>
    {
        private Sistema Db { get; set; }
        private Pedido RPedido { get; set; }
        private IMapper Mapper { get; set; }

        public PedidoCore(IMapper mapper)
        {
            //Ler arquivo
            Db = file.ManipulacaoDeArquivos(null).sistema;

            //Caso arquivo não existe 
            Db = Db ?? new Sistema();

            //Intanciando o mapper
            Mapper = mapper;
        }

        public PedidoCore(PedidoView pedido,IMapper mapper)
        {
            //Ler arquivo
            Db = file.ManipulacaoDeArquivos(null).sistema;

            //Caso arquivo não existe 
            Db = Db ?? new Sistema();

            //Intanciando o mapper
            Mapper = mapper;

            RPedido = Mapper.Map<Pedido>(pedido);

            //Preenchendo informações do cliente
            RPedido._cliente = Mapper.Map<Cliente>(Db.Clientes.SingleOrDefault(temp => temp.Id == RPedido._cliente.Id));

            //Preenchendo informações dos produtos
            RPedido.Compras.ForEach(c => c.Copiar(Db.Produtos.SingleOrDefault(temp => temp.Id == c.Id)));

            RuleFor(e => e._cliente)
                .NotNull()
                .NotEmpty()
                .WithMessage("Cliente não existe na base de dados");

            RuleFor(e => e.Compras)
                .NotNull()
                .NotEmpty()
                .ForEach(e => e.Must(ValidaLista).WithMessage("O produto nâo existe na base de dados ou Quantidade ultrapassa o estoque"));

            RuleForEach(e => e.Compras)
                .Must(p => p.Quantidade> 0)
                .WithMessage("Quantidade Invalida");
        }

        /// <summary>
        /// Cadastro de um pedido 
        /// </summary>
        /// <returns></returns>
        
        public Retorno CadastroPedido()
        {

            var results = Validate(RPedido);

            // Se o modelo é inválido retorno false
            if (!results.IsValid)
                return new Retorno { Status = false, Resultado = results.Errors.Select(c=>c.ErrorMessage) };

            //Calculo valor total
            RPedido.ValorTotal=decimal.Parse(RPedido.ValorTotalAPagar());

            //retirada da quantidade dos produtos do estoque 
            RPedido.Compras.ForEach(c => Db.Produtos.SingleOrDefault(d => d.Id == c.Id).Quantidade -= c.Quantidade);

            //Adiciona o pedido ao "banco"
            Db.Pedidos.Add(RPedido);

            //Reescreve arquivo
            file.ManipulacaoDeArquivos(Db);

            return new Retorno() { Status = true, Resultado =Mapper.Map<PedidoViewRetorno>(RPedido) };
        }

        /// <summary>
        /// Retorna todos os elementos da lista Pedido
        /// </summary>
        /// <returns></returns>

        public Retorno Lista()
        {
            var resposta = Mapper.Map<List<PedidoViewRetorno>>(Db.Pedidos.OrderBy(x => x._cliente.Nome));

            return resposta.Count != 0 ?
                     new Retorno { Status = true, Resultado = resposta } :
                     new Retorno { Status = false, Resultado = "Não existe consulta" };
        }

        /// <summary>
        /// Busca um Pedido pelos parametros passados
        /// </summary>
        /// <param name="id">string passada para procurar por id</param>
        /// <returns></returns>
        
        public Retorno ID(string id)
        {
            try{
                return new Retorno { Status = true, Resultado = Mapper.Map<PedidoViewRetorno>(Db.Pedidos.Single(x => x.Id == Guid.Parse(id))) };
            }
            catch (Exception) { }
            return new Retorno { Status = false, Resultado = "Pedido não existe" };
        }

        /// <summary>
        /// Deleta um pedido pela id passado
        /// </summary>
        /// <param name="id">parametro passado para achar o pedido</param>
        /// <returns></returns>

        public Retorno DeletaPedido(string id)
        {
            try {
                RPedido = Db.Pedidos.Single(c => c.Id == Guid.Parse(id));

                Db.Pedidos.Remove(RPedido);

                //Reescreve arquivo 
                file.ManipulacaoDeArquivos(Db);

                return new Retorno { Status = true, Resultado = "Pedido Apagado" };
            }
            catch (Exception) { }
            return new Retorno { Status = false, Resultado = "Pedido não existe" };
        }

        /// <summary>
        /// Consulta os Pedido
        /// </summary>
        /// <param name="NumeroPagina">É para controlar a pagina que esta sendo mostrada</param>
        /// <param name="Ordem">É para definir se estara por nome ou data</param>
        /// <param name="TamanhoPagina">Tamanho da pagina</param>
        /// <returns>Retorna uma lista de Pedido</returns>
        
        public Retorno PorPagina(int NumeroPagina, string Ordem, int TamanhoPagina)
        {
            //limite de elementos por paginas
            if (TamanhoPagina > 30) TamanhoPagina = 30;

            try
            {
                var Pagina = CalculoPaginas(TamanhoPagina);

                if (Ordem.ToLower() == "Valor")
                    return new Retorno { Status = true, Resultado = (Mapper.Map<List<PedidoViewRetorno>>(Db.Pedidos.OrderBy(x => x.ValorTotal).Skip((NumeroPagina - 1) * TamanhoPagina).Take(TamanhoPagina)), $"Total de paginas: {Pagina}") };

                return Ordem.ToLower() == "Nome" && NumeroPagina >= 1 ?
                      new Retorno { Status = true, Resultado = (Mapper.Map<List<PedidoViewRetorno>>(Db.Pedidos.OrderBy(x => x._cliente.Nome).Skip((NumeroPagina - 1) * TamanhoPagina).Take(TamanhoPagina)), $"Total de paginas: {Pagina}") } :
                      new Retorno { Status = true, Resultado = (Mapper.Map<List<PedidoViewRetorno>>(Db.Pedidos.Skip((NumeroPagina - 1) * TamanhoPagina).Take(TamanhoPagina)), $"Total de paginas: {Pagina}") };
            }
            catch (Exception){}

            //se paginação é não é possivel
            return new Retorno { Status = true, Resultado = (Mapper.Map<List<PedidoViewRetorno>>(Db.Pedidos.OrderBy(x => x.DataCadastro).Take(15)), $"Total de paginas: {CalculoPaginas(15)}") };
        }

        /// <summary>
        /// Retorna uma lista refinada por data
        /// </summary>
        /// <param name="DataInicial">Parametro para refinar a busca por uma data inicial</param>
        /// <param name="DataFinal">Parametro para refinar a busca por uma data Final</param>
        /// <returns></returns>
        
        public Retorno PorData(string DataInicial, string DataFinal)
        {

            if (!DateTime.TryParse(DataInicial, out DateTime date) && !DateTime.TryParse(DataFinal, out DateTime time) )
                return new Retorno { Status = false, Resultado = "Dados Invalidos" };

            //Caso Data final seja nula ou errada
            if (!DateTime.TryParse(DataFinal, out time))
                return new Retorno { Status = true, Resultado = Mapper.Map<List<PedidoViewRetorno>>(Db.Pedidos.Where(x => x.DataCadastro >= date)) };

            //Caso Data inicial seja nula ou errada
            return !DateTime.TryParse(DataInicial, out date)?
                new Retorno { Status = true, Resultado = Mapper.Map<List<PedidoViewRetorno>>(Db.Pedidos.Where(x => x.DataCadastro <= time)) }:
                new Retorno { Status = true, Resultado =Mapper.Map<List<PedidoViewRetorno>>(Db.Pedidos.Where(x => x.DataCadastro >= date && x.DataCadastro <= time)) };
        }       
        
        //Valida a lista de produtos
        public bool ValidaLista(Produto produto)
        {
            var tempProd = Db.Produtos.SingleOrDefault(e => e.Id == produto.Id);
            if (produto==null) return false;
            return produto.Quantidade>tempProd.Quantidade? false : true;
        }

        /// <summary>
        /// Realiza o calculo para saber quantas paginas existem
        /// </summary>
        /// <param name="TamanhoPagina">Divisão das paginas</param>
        /// <returns></returns>
       
        public int CalculoPaginas(int TamanhoPagina)
        {
            //Calculo para paginas
            var lista = Db.Pedidos.Count;
            var Paginas = lista / TamanhoPagina;
            return lista % TamanhoPagina != 0 ? Paginas += 1 : Paginas;
        }
    }
}
