using Carter;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Serilog;
using shared.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((hostingContext, configuration) =>
{
    configuration.ReadFrom.Configuration(hostingContext.Configuration);
});

builder.Services
    .AddCatalogModule(builder.Configuration)
    .AddOrderModule(builder.Configuration)
    .AddBasketModule(builder.Configuration);

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Redis");
});

builder.Services.AddCarterWithAssemblies(
    typeof(CatalogModule).Assembly,
    typeof(BasketModule).Assembly);

var app = builder.Build();


DatabaseInitializer(app);

app.MapGet("/", () => "Hello World!");
app.MapCarter();
app.UseSerilogRequestLogging();
app.UseExceptionHandler(exceptionHandlerApp =>
{
    exceptionHandlerApp.Run(async context =>
    {
        var exception = context.Features.Get<IExceptionHandlerFeature>()?.Error;
        if (exception is null)
        {
            return;
        }

        var problemDetails = new ProblemDetails
        {
            Title = exception.Message,
            Status = StatusCodes.Status500InternalServerError,
            Detail = exception.StackTrace
        };

        var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
        logger.LogError(exception, message: exception.Message);

        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        context.Response.ContentType = "application/problem+json";

        await context.Response.WriteAsJsonAsync(problemDetails);
    });
});

app.Run();

static async void DatabaseInitializer(WebApplication app)
{
    using var scope = app.Services.CreateScope();

    #region Migrate Modules Database

    // Catalog Module
    var catalogDbContext = scope.ServiceProvider.GetRequiredService<catalog.Data.CatalogDbContext>();
    await catalogDbContext.Database.EnsureCreatedAsync();
    await catalogDbContext.Database.MigrateAsync();

    // Basket Module 
    var basketDbContext = scope.ServiceProvider.GetRequiredService<basket.Data.BasketDbContext>();
    await basketDbContext.Database.EnsureCreatedAsync();
    await basketDbContext.Database.MigrateAsync();

    #endregion

    #region Seed Data

    var seeders = scope.ServiceProvider.GetServices<shared.Data.Seed.IDataSeeder>();
    foreach(var seeder in seeders)
    {
        await seeder.SeedAsync();
    }

    #endregion
}