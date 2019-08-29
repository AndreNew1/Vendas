namespace Model
{
    public class Produto : Base
    {
        public string Nome { get; set; }
        public decimal Valor { get; set; }
        public int Quantidade { get; set; }

        public decimal ValorTotal()=> Valor * Quantidade;
     
        public void Trocar(Produto produto)
        {
            Nome = produto.Nome;
            Valor = produto.Valor;
        }
    }
}
