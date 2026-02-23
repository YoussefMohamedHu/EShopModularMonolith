using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;
using order.Orders.Dtos;
using order.Orders.Features.CreateOrder;
using Shared.Messaging.IntegrationEvents;

namespace order.Orders.EventHandlers
{
    public class BasketCheckoutIntegrationEventHandler(
        ISender sender,
        ILogger<BasketCheckoutIntegrationEventHandler> logger)
        : IConsumer<BasketCheckoutIntegrationEvent>
    {
        public async Task Consume(ConsumeContext<BasketCheckoutIntegrationEvent> context)
        {
            var @event = context.Message;

            logger.LogInformation("Integration Event handled: {IntegrationEvent}", @event.GetType().Name);

            var orderItems = @event.Items.Select(item => new OrderItemDto
            {
                ProductId = item.ProductId,
                ProductName = item.ProductName,
                Price = item.Price,
                Quantity = item.Quantity
            }).ToList();

            var orderDto = new OrderDto
            {
                CustomerId = @event.CustomerId,
                OrderName = $"{@event.UserName}_{@event.EventId}",
                ShippingAddress = new AddressDto
                {
                    EmailAddress = @event.EmailAddress,
                    AddressLine = @event.AddressLine,
                    City = @event.City,
                    Country = @event.Country
                },
                Payment = new PaymentDto
                {
                    CardName = @event.CardName,
                    CardNumber = @event.CardNumber,
                    ExpirationDate = @event.ExpirationDate,
                    CVV = @event.CVV
                },
                Items = orderItems,
                TotalPrice = @event.TotalPrice
            };

            var result = await sender.Send(new CreateOrderCommand(orderDto), context.CancellationToken);

            logger.LogInformation("Order created with Id: {OrderId} for user: {UserName}", result.Id, @event.UserName);
        }
    }
}
