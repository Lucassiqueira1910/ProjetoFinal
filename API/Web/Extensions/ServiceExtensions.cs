using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using USUARIOminimalSolution.Application.Services;
using USUARIOminimalSolution.Infrastructure.Persistence;
using USUARIOminimalSolution.Infrastructure.Repositories;

namespace USUARIOminimalSolution.Web.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddProjectServices(this IServiceCollection services, IConfiguration configuration)
        {
            
            var connString = File.ReadAllText("connection.txt").Trim();

           
            services.AddDbContext<ApplicationDbContext>(opts =>
                opts.UseOracle(connString));

            services.AddScoped<IUsuarioRepository, UsuarioRepository>();
            services.AddScoped<UsuarioService>();

            return services;
        }
    }
}