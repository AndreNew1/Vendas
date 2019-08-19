using System;

namespace Model
{
    public abstract class Base
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public DateTime DataCadastro { get; set; } = DateTime.Now;
    }
}
