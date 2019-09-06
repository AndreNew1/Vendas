using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Swagger;
using Microsoft.AspNetCore.Rewrite;
using AutoMapper;
using Model;

namespace ApiVendas
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var Config = new MapperConfiguration(cfg =>
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
                .ForMember(d => d.Nome, opts => opts.Condition(src => src.Nome != null&&src.Nome.Length>3))
                .ForMember(d => d.CPF, opts => opts.Condition(src => src.CPF != null&&src.CPF.Length==11));

                cfg.CreateMap<Produto, Produto>()
                .ForMember(d => d.DataCadastro, opts => opts.Ignore())
                .ForMember(d => d.Id, opts => opts.Ignore())
                .ForMember(d => d.Nome, opts => opts.Condition(src => src.Nome != null && src.Nome.Length > 3))
                .ForMember(d => d.Quantidade, opts => opts.Condition(src => src.Quantidade > 0))
                .ForMember(d => d.Valor, opts => opts.Condition(src => src.Valor > 0));

                cfg.CreateMap<PedidoView, Pedido>()
                .ForPath(d => d._cliente.Id, opts => opts.MapFrom(s => s.ClienteId));
                
                cfg.CreateMap<ComprasView, Produto>();

                cfg.CreateMap<Pedido, PedidoViewRetorno>()
                .ForMember(d => d.DataCadastro, opts => opts.MapFrom(s => s.DataCadastro.ToString("dd/MMMM/yyyy")));
            });

            IMapper mapper = Config.CreateMapper();
            services.AddSingleton(mapper);
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            services.AddSwaggerGen(c =>
            {

                c.SwaggerDoc("v1", new Info { Title = "Vendas" });

            });
            
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {

                c.SwaggerEndpoint("/swagger/v1/swagger.json", "VENDAS");

            });

            var option = new RewriteOptions();
            option.AddRedirect("^$", "swagger");
            app.UseRewriter(option);

            app.UseHttpsRedirection();
            app.UseMvc();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                  name: "",
                  template: "{controller=Values}/{id?}");
            });
        }
    }
}
