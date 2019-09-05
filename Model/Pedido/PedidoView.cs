using System;
using System.Collections.Generic;

namespace Model
{
    public class PedidoView
    {
        public Guid ClienteId { get; set; }
        public List<ComprasView> Compras { get; set; }
    }
}
