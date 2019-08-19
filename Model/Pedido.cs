using System;
using System.Collections.Generic;
using System.Text;

namespace Model
{
    public class Pedido:Base
    {
        public Cliente _cliente { get; set; }
        public List<Produto> Compras { get; set; }
        public decimal ValorTotal { get; set; } 
    }
}
