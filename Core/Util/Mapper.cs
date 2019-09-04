using AutoMapper;
using Model;

namespace Core.Util
{
    public class NewMapper
    {
        public MapperConfiguration Config { get; set; }
        public NewMapper()
        {
            Config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<ClienteView, Cliente>()
                .ForMember(d => d.Nome, opts => opts.Condition(src => src.Nome != null))
                .ForMember(d => d.CPF, opts => opts.Condition(src => src.CPF != null));
                cfg.CreateMap<Cliente, ClienteViewRetorno>()
                .ForMember(d => d.DataCadastro, opts => opts.MapFrom(s => s.DataCadastro.ToString("dd/MMMM/yyyy")));
                cfg.CreateMap<ProdutoView, Produto>();
                cfg.CreateMap<Produto, ProdutoViewRetorno>()
                .ForMember(d => d.DataCadastro, opts => opts.MapFrom(s => s.DataCadastro.ToString("dd/MMMM/yyyy")));
                cfg.CreateMap<Cliente, Cliente>()
                .ForMember(d => d.DataCadastro, opts => opts.Ignore())
                .ForMember(d => d.Id, opts => opts.Ignore())
                .ForMember(d => d.Nome, opts => opts.Condition(src => src.Nome != null))
                .ForMember(d => d.CPF, opts => opts.Condition(src => src.CPF != null));
                cfg.CreateMap<Produto, Produto>()
                .ForMember(d => d.DataCadastro, opts => opts.Ignore())
                .ForMember(d => d.Id, opts => opts.Ignore())
                .ForMember(d => d.Nome, opts => opts.Condition(src => src.Nome != null))
                .ForMember(d => d.Quantidade, opts => opts.Condition(src => src.Quantidade > 0))
                .ForMember(d => d.Valor, opts => opts.Condition(src => src.Valor > 0));
                cfg.CreateMap<PedidoView, Pedido>()
                .ForMember(d => d._cliente, opts => opts.MapFrom(s => new Cliente { Id = s._clienteId }));
                cfg.CreateMap<ComprasView, Produto>();
                cfg.CreateMap<Pedido, PedidoViewRetorno>()
                .ForMember(d => d.DataCadastro, opts => opts.MapFrom(s => s.DataCadastro.ToString("dd/MMMM/yyyy")));
            });
        }
    }
}
