using Microsoft.EntityFrameworkCore;
using shared.Data.Seed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace catalog.Data.Seeds
{
    public class CatalogDataSeeder(CatalogDbContext dbContext) : IDataSeeder
    {
        public async Task SeedAsync()
        {
            if(!await dbContext.Products.AnyAsync())
            {
                dbContext.Products.AddRange(InitialSeed.Proudct());
                    
                await dbContext.SaveChangesAsync();
            }
        }
    }
}
