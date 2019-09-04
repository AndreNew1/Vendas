using System.Collections.Generic;

namespace Model
{
    public class Pedido:Base
    {
        public Cliente _cliente { get; set; }
        public List<Produto> Compras { get; set; }
        public decimal ValorTotal { get; set; } 

        public string ValorTotalAPagar()
        {
            //Recebe o valor total de todos os itens da lista
            Compras.ForEach(e => ValorTotal += e.ValorTotal());
            if (ValorTotal >= 300) return (ValorTotal -= ValorTotal * 0.10M).ToString("F2");
            return ValorTotal >= 100 ? (ValorTotal -= ValorTotal * 0.05M).ToString("F2") : ValorTotal.ToString("F2");
        }
    }
}
