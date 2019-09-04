using System;
using System.Collections.Generic;

namespace Model
{
    public class PedidoView
    {
        public Guid _clienteId { get; set; }
        public List<ComprasView> Compras { get; set; }
    }
}
