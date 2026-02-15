using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Base.Data.Seed
{
    public interface IDataSeeder
    {
        Task SeedAsync();
    }
}
