using order.Orders.Models;
using Shared.Base.DDD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace order.Orders.Events
{
    public record OrderCreatedEvent(Order order) : IDomainEvent;
}
