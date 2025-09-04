using Microsoft.Extensions.DependencyInjection;
using ThomTwo.Domain.Repository;
using ThomTwo.Infrasctructure.Persistence.Repositories;

namespace ThomTwo.Infrasctructure.Persistence.Configurations;

    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services)
        {
            services.AddScoped<IOfficerRepository, OfficerRepository>();
            return services;
        }
    }

