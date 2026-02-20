using MediatR;
using Microsoft.EntityFrameworkCore;
using order.Data;
using order.Orders.Dtos;
using order.Orders.Models;
using Shared.Base.Pagination;

namespace order.Orders.Features.GetOrders
{
    public record GetOrdersQuery(PaginationRequest Request) : IRequest<GetOrdersResult>;
    public record GetOrdersResult(PaginatedResult<OrderDto> Orders);

    public class GetOrdersHandler(OrderDbContext dbContext) : IRequestHandler<GetOrdersQuery, GetOrdersResult>
    {
        public async Task<GetOrdersResult> Handle(GetOrdersQuery query, CancellationToken cancellationToken)
        {
            int pageIndex = query.Request.PageIndex;
            int pageSize = query.Request.PageSize;
            int totalCount = await dbContext.Orders.CountAsync(cancellationToken);

            var orders = await dbContext.Orders
                .AsNoTracking()
                .Include(o => o.Items)
                .OrderBy(o => o.OrderName)
                .Skip(pageIndex * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            var orderDtos = orders.Select(MapOrder).ToList();

            return new GetOrdersResult(
                new PaginatedResult<OrderDto>(
                    pageIndex,
                    pageSize,
                    totalCount,
                    orderDtos));
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
