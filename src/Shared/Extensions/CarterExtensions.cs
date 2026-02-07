using Carter;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace shared.Extensions
{
    public static class CarterExtensions
    {
        public static IServiceCollection AddCarterWithAssemblies (this IServiceCollection services, params Assembly[] assemblies)
        {
            services.AddCarter(configurator : config =>
            {
                foreach (var assembly in assemblies)
                {
                    var moduels = assembly.GetTypes()
                        .Where(t => t.IsAssignableTo(typeof(ICarterModule))).ToArray();

                    config.WithModules(moduels);
                }
            });
            return services;
        }
    }
}
