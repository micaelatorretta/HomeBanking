using HomeBanking.Models;
using HomeBanking.Repositories.Interfaces;
using HomeBanking.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using System;
using HomeBanking.Controllers;

namespace HomeBanking
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }


        public void ConfigureServices(IServiceCollection services)
        {
            // Agrega el servicio de Razor Pages
            services.AddRazorPages();

            // Agrega el servicio de controladores y configura las opciones de serialización JSON
            services.AddControllers().AddJsonOptions(x => x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve);

            // Agrega el contexto de base de datos usando SQL Server y la cadena de conexión obtenida de la configuración
            services.AddDbContext<HomeBankingContext>(opt => opt.UseSqlServer(Configuration.GetConnectionString("HomeBankingConexion")));

            // Agrega un servicio de repositorio para operaciones relacionadas con clientes
            services.AddScoped<IClientRepository, ClientRepository>();

            // Agrega un servicio de repositorio para operaciones relacionadas con cuentas
            services.AddScoped<IAccountRepository, AccountRepository>();
            // Agrega un servicio de repositorio para operaciones relacionadas con TARJETAS
            services.AddScoped<ICardRepository, CardRepository>();

            services.AddScoped<AccountsController>();
            services.AddScoped<CardsController>();


            // Configura la autenticación usando cookies
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.ExpireTimeSpan = TimeSpan.FromMinutes(10);
                    options.LoginPath = new PathString("/index.html");
                });

            // Configura una política de autorización que requiere el claim "Client"
            services.AddAuthorization(options =>
            {
                options.AddPolicy("ClientOnly", policy => policy.RequireClaim("Client"));
            });

        }


        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // Comprueba si la aplicación está en modo de desarrollo
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // Si no está en modo de desarrollo, utiliza una página de error personalizada
                app.UseExceptionHandler("/Error");
            }

            // Permite servir archivos estáticos (por ejemplo, CSS, JavaScript, imágenes)
            app.UseStaticFiles();

            // Configura el enrutamiento de las solicitudes
            app.UseRouting();

            // Agrega middleware de autenticación
            app.UseAuthentication();

            // Agrega middleware de autorización
            app.UseAuthorization();

            // Configura los endpoints para el enrutamiento
            app.UseEndpoints(endpoints =>
            {
                // Mapea las Razor Pages a los endpoints
                endpoints.MapRazorPages();

                // Mapea los controladores a los endpoints
                endpoints.MapControllers();
            });
        }

    }
}
