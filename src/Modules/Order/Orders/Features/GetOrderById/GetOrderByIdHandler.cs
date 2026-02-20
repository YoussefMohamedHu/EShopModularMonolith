using MediatR;
using Microsoft.EntityFrameworkCore;
using order.Data;
using order.Orders.Dtos;
using order.Orders.Models;

namespace order.Orders.Features.GetOrderById
{
    public record GetOrderByIdQuery(Guid OrderId) : IRequest<GetOrderByIdResult>;
    public record GetOrderByIdResult(OrderDto Order);

    public class GetOrderByIdHandler(OrderDbContext dbContext) : IRequestHandler<GetOrderByIdQuery, GetOrderByIdResult>
    {
        public async Task<GetOrderByIdResult> Handle(GetOrderByIdQuery query, CancellationToken cancellationToken)
        {
            var order = await dbContext.Orders
                .AsNoTracking()
                .Include(o => o.Items)
                .SingleOrDefaultAsync(o => o.Id == query.OrderId, cancellationToken);

            if (order is null)
            {
                throw new Exception($"Order with id {query.OrderId} not found.");
            }

            return new GetOrderByIdResult(MapOrder(order));
        }

        private static OrderDto MapOrder(Order order)
        {
            return new OrderDto
            {
                Id = order.Id,
                CustomerId = order.CustomerId,
                OrderName = order.OrderName,
                ShippingAddress = new AddressDto
                {
                    EmailAddress = order.ShippingAddress.EmailAddress,
                    AddressLine = order.ShippingAddress.AddressLine,
                    City = order.ShippingAddress.City,
                    Country = order.ShippingAddress.Country
                },
                Payment = new PaymentDto
                {
                    CardName = order.Payment.CardName,
                    CardNumber = order.Payment.CardNumber,
                    ExpirationDate = order.Payment.ExpirationDate,
                    CVV = order.Payment.CVV
                },
                Items = order.Items.Select(MapItem).ToList(),
                TotalPrice = order.TotalPrice
            };
        }

        private static OrderItemDto MapItem(OrderItem item)
        {
            return new OrderItemDto
            {
                Id = item.Id,
                OrderId = item.OrderId,
                ProductId = item.ProductId,
                ProductName = item.ProductName,
                Price = item.Price,
                Quantity = item.Quantity
            };
        }
    }
}
