using System;
using System.Collections.Generic;
using System.Text;

namespace Model
{
    public class ProdutoViewRetorno
    {
        public Guid Id { get; set; }
        public string DataCadastro { get; set; }
        public string Nome { get; set; }
        public decimal Valor { get; set; }
        public int Quantidade { get; set; }
    }
}
