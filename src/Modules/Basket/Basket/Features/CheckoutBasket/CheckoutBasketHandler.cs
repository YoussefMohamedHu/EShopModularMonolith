using basket.Basket.Dtos;
using basket.Basket.Models;
using basket.Data;
using FluentValidation;
using MassTransit;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Modules.Basket.Data.Repositories;
using Shared.Messaging.IntegrationEvents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace basket.Basket.Features.CheckoutBasket
{
    public record CheckoutBasketCommand(BasketCheckoutDto Basket) : IRequest<CheckoutBasketResult>;
    public record CheckoutBasketResult(bool IsSuccess);

    public class CheckoutBasketCommandValidator : AbstractValidator<CheckoutBasketCommand>
    {
        public CheckoutBasketCommandValidator()
        {
            RuleFor(c => c.Basket.CustomerId)
                .NotEmpty().WithMessage("Customer id is required.");
            RuleFor(c => c.Basket).NotNull()
                .WithMessage("Basket is required.");

        }
    }
    public class CheckoutBasketHandler(
        BasketDbContext _dbContext,
        ILogger<CheckoutBasketHandler> logger
        ) : IRequestHandler<CheckoutBasketCommand, CheckoutBasketResult>
    {
        public async Task<CheckoutBasketResult> Handle(CheckoutBasketCommand command, CancellationToken cancellationToken)
        {
            await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

            try
            {
                var basket = await _dbContext.ShoppingCarts
                    .Include(b => b.Items)
                    .SingleOrDefaultAsync(x => x.UserName == command.Basket.UserName, cancellationToken);


                if (basket == null)
                {
                    throw new Exception($"Basket not found for user {command.Basket.UserName}");
                }

                var eventMessage = new BasketCheckoutIntegrationEvent
                {
                    UserName = command.Basket.UserName,
                    CustomerId = command.Basket.CustomerId,
                    TotalPrice = basket.TotalPrice,
                    Items = basket.Items.Select(i => new BasketItemEventDto
                    {
                        ProductId = i.ProductId,
                        ProductName = i.ProductName,
                        Price = i.Price,
                        Quantity = i.Quantity
                    }).ToList(),
                    CardName = command.Basket.CardName,
                    CardNumber = command.Basket.CardNumber,
                    ExpirationDate = command.Basket.ExpirationDate,
                    CVV = command.Basket.CVV,
                    EmailAddress = command.Basket.EmailAddress,
                    AddressLine = command.Basket.AddressLine,
                    City = command.Basket.City,
                    Country = command.Basket.Country
                };

                var outboxMessage = new OutboxMessage
                    (typeof(BasketCheckoutIntegrationEvent).AssemblyQualifiedName!,
                   System.Text.Json.JsonSerializer.Serialize(eventMessage));

                _dbContext.OutboxMessages.Add(outboxMessage);
                _dbContext.ShoppingCarts.Remove(basket);

                await _dbContext.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);

                return new CheckoutBasketResult(true);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync(cancellationToken);
                logger.LogError(ex, "Error occurred while checking out basket for user {UserName}", command.Basket.UserName);
                return new CheckoutBasketResult(false);
            }
        }
    }
}
