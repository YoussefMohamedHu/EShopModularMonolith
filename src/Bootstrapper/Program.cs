using Carter;
using Microsoft.EntityFrameworkCore;
using shared.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddCatalogModule(builder.Configuration)
    .AddOrderModule(builder.Configuration)
    .AddBasketModule(builder.Configuration);

builder.Services.AddCarterWithAssemblies(typeof(CatalogModule).Assembly);

var app = builder.Build();

DatabaseInitializer(app);

app.MapGet("/", () => "Hello World!");
app.MapCarter();

app.Run();

static async void DatabaseInitializer(WebApplication app)
{
    using var scope = app.Services.CreateScope();

    #region Migrate Modules Database

    // Catalog Module
    var catalogDbContext = scope.ServiceProvider.GetRequiredService<catalog.Data.CatalogDbContext>();
    await catalogDbContext.Database.EnsureCreatedAsync();
    await catalogDbContext.Database.MigrateAsync();

    #endregion

    #region Seed Data

    var seeders = scope.ServiceProvider.GetServices<shared.Data.Seed.IDataSeeder>();
    foreach(var seeder in seeders)
    {
        await seeder.SeedAsync();
    }

    #endregion
}