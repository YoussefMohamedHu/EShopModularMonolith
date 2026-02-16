using basket.Basket.Features.UpdateItemPriceInBasket;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;
using Shared.Messaging.IntegrationEvents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace basket.Basket.EventHandlers
{
    public class ProductPriceChangedIntegrationEventHandler
        (ISender sender,
        ILogger<ProductPriceChangedIntegrationEventHandler> logger)
        : IConsumer<ProductPriceChangedIntegrationEvent>
    {
        public async Task Consume(ConsumeContext<ProductPriceChangedIntegrationEvent> context)
        {
            logger.LogInformation("Integration Event handled: {IntegrationEvent}", context.Message.GetType().Name);

            var command = new UpdateItemPriceInBasketCommand(context.Message.ProductId, context.Message.Price);

            var result = await sender.Send(command);

            if (!result.IsSuccess)
            {
                logger.LogError("Faild to update product price with Id: {ProductId}", context.Message.ProductId);
            }

            logger.LogInformation("Product Price with Id: {ProductId}, updated successfully", context.Message.ProductId);
        }
    }
}
