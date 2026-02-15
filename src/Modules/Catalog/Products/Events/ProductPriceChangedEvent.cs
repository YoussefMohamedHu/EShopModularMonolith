using catalog.Products.Models;
using Shared.Base.DDD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace catalog.Products.Events
{
    public record ProductPriceChangedEvent(Product Product) : IDomainEvent;
    
}
