using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using shared.Data.Seed;
namespace shared.Data
{
    public static class Extensions
    {
        public static async Task SeedDataAsync(IServiceProvider serviceProvider)
        {
            var scope = serviceProvider.CreateScope();
            var seeders = scope.ServiceProvider.GetServices<IDataSeeder>();
            foreach(var seeder in seeders)
            {
                await seeder.SeedAsync();
            }
        }
       
    }
}
