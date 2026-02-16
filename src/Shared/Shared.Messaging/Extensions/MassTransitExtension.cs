using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Shared.Messaging.Extensions
{
    public static class MassTransitExtension
    {
        public static IServiceCollection AddMassTransitWithAssemblies
            (this IServiceCollection services, IConfiguration configuration, params Assembly[] assemblies)
        {
            services.AddMassTransit(configure =>
            {
                configure.SetKebabCaseEndpointNameFormatter();
                configure.SetInMemorySagaRepositoryProvider();

                configure.AddConsumers(assemblies);
                configure.AddSagaStateMachines(assemblies);
                configure.AddSagas(assemblies);
                configure.AddActivities(assemblies);

                /*configure.UsingInMemory((context, inMemoryCfg) =>
                {
                    inMemoryCfg.ConfigureEndpoints(context);
                });*/

                configure.UsingRabbitMq((context, rabbitCfg) =>
                {
                    rabbitCfg.Host(new Uri(configuration["MessageBroker:Host"]!), host =>
                    {
                        host.Username(configuration["MessageBroker:Username"]!);
                        host.Password(configuration["MessageBroker:Password"]!);
                    });
                    rabbitCfg.ConfigureEndpoints(context);
                });
            });
            return services;
        }
    }
}
