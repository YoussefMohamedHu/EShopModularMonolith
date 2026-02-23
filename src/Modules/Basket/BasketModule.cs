using System.Reflection;
using basket.Data;
using basket.Data.Processors;
using basket.Data.Repositories;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Modules.Basket.Data.Repositories;
using Shared.Base.Behaviors;
using Shared.Base.Data.Interceptors;
using Shared.Base.Data.Seed;

namespace EShopModularMonolith.Modules.Basket;

public static class BasketModule
{
    public static IServiceCollection AddBasketModule(this IServiceCollection services,
        IConfiguration configuration)
    {
        #region Application
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
            cfg.AddOpenBehavior(typeof(LoggingBehavior<,>));
        });

        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        services.AddScoped<IBasketRepository, BasketRepository>();
        services.Decorate<IBasketRepository, CachedBasketRepository>();
        #endregion

        #region Infrastructure

        var connectionString = configuration.GetConnectionString("DefaultConnection");

        services.AddScoped<ISaveChangesInterceptor, AuditableEntityInterceptor>();
        services.AddScoped<ISaveChangesInterceptor, DispatchDomainEventsInterceptor>();

        

        services.AddDbContext<BasketDbContext>((sp, options) =>
        {
            options.AddInterceptors(sp.GetServices<ISaveChangesInterceptor>());
            options.UseNpgsql(connectionString);
        });

        services.AddHostedService<OutboxProcessor>();

        //services.AddScoped<IDataSeeder, BasketDataSeeder>();

        #endregion
        return services;
    }
}
