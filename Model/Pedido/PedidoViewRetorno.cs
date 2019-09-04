using System;
using System.Collections.Generic;
using System.Text;

namespace Model
{
    public class PedidoViewRetorno
    {
        public Guid Id { get; set; }
        public string DataCadastro { get; set; }
        public Cliente _cliente { get; set; }
        public List<Produto> Compras { get; set; }
        public decimal ValorTotal { get; set; }
    }
}
