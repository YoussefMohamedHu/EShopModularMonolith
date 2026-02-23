using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using order.Data;
using order.Orders.Dtos;
using order.Orders.Models;
using order.Orders.ValueObjects;

namespace order.Orders.Features.CreateOrder
{
    public record CreateOrderCommand(OrderDto Order) : IRequest<CreateOrderResult>;
    public record CreateOrderResult(Guid Id);

    public class CreateOrderCommandValidator : AbstractValidator<CreateOrderCommand>
    {
        public CreateOrderCommandValidator()
        {
            RuleFor(c => c.Order.CustomerId)
                .NotEmpty().WithMessage("Customer id is required.");
            RuleFor(c => c.Order.OrderName)
                .NotEmpty().WithMessage("Order name is required.");

            RuleFor(c => c.Order.ShippingAddress)
                .NotNull().WithMessage("Shipping address is required.");

            When(c => c.Order.ShippingAddress is not null, () =>
            {
                RuleFor(c => c.Order.ShippingAddress!.AddressLine)
                    .NotEmpty().WithMessage("Shipping address line is required.");
                RuleFor(c => c.Order.ShippingAddress!.City)
                    .NotEmpty().WithMessage("Shipping city is required.");
                RuleFor(c => c.Order.ShippingAddress!.Country)
                    .NotEmpty().WithMessage("Shipping country is required.");
            });

            RuleFor(c => c.Order.Payment)
                .NotNull().WithMessage("Payment is required.");

            When(c => c.Order.Payment is not null, () =>
            {
                RuleFor(c => c.Order.Payment!.CardName)
                    .NotEmpty().WithMessage("Card name is required.");
                RuleFor(c => c.Order.Payment!.CardNumber)
                    .NotEmpty().WithMessage("Card number is required.");
                RuleFor(c => c.Order.Payment!.ExpirationDate)
                    .NotEmpty().WithMessage("Card expiration date is required.");
                RuleFor(c => c.Order.Payment!.CVV)
                    .NotEmpty().WithMessage("Card CVV is required.");
            });

            RuleFor(c => c.Order.Items)
                .NotNull().WithMessage("Order items are required.")
                .Must(items => items?.Count > 0).WithMessage("Order must contain at least one item.");

            RuleForEach(c => c.Order.Items).ChildRules(items =>
            {
                items.RuleFor(item => item.ProductId)
                    .NotEmpty().WithMessage("Product id is required.");
                items.RuleFor(item => item.ProductName)
                    .NotEmpty().WithMessage("Product name is required.");
                items.RuleFor(item => item.Price)
                    .GreaterThan(0).WithMessage("Item price must be greater than zero.");
                items.RuleFor(item => item.Quantity)
                    .GreaterThan(0).WithMessage("Item quantity must be greater than zero.");
            });
        }
    }

    public class CreateOrderHandler(OrderDbContext dbContext,ILogger<CreateOrderHandler> logger) : IRequestHandler<CreateOrderCommand, CreateOrderResult>
    {
        public async Task<CreateOrderResult> Handle(CreateOrderCommand command, CancellationToken cancellationToken)
        {
            var orderDto = command.Order;

            var shippingAddress = Address.Of(
                orderDto.ShippingAddress.EmailAddress,
                orderDto.ShippingAddress.AddressLine,
                orderDto.ShippingAddress.City,
                orderDto.ShippingAddress.Country);

            var payment = Payment.Of(
                orderDto.Payment.CardName,
                orderDto.Payment.CardNumber,
                orderDto.Payment.ExpirationDate,
                orderDto.Payment.CVV);

            var order = Order.Create(
                Guid.NewGuid(),
                orderDto.CustomerId,
                orderDto.OrderName,
                shippingAddress,
                shippingAddress,
                payment);

            foreach (var item in orderDto.Items)
            {
                order.AddItem(order.Id, item.ProductId, item.ProductName, item.Price, item.Quantity);
            }

            logger.LogInformation("Creating order for customer {CustomerId} with {ItemCount} items", order.CustomerId, order.Items.Count);

            dbContext.Orders.Add(order);

            await dbContext.SaveChangesAsync(cancellationToken);

            return new CreateOrderResult(order.Id);
        }
    }
}
