using catalog.Products.Events;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace catalog.Products.EventHandlers
{
    public class ProductCreatedEventHandler(ILogger<ProductCreatedEvent> logger) : INotificationHandler<ProductCreatedEvent>
    {
        public Task Handle(ProductCreatedEvent notification, CancellationToken cancellationToken)
        {
            logger.LogInformation(
                "Domain Event Handled: {DomainEvent}",notification.GetType().Name
            );
            return Task.CompletedTask;
        }
    }
}
