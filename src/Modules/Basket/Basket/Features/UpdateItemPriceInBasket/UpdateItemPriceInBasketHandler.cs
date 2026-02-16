using basket.Data;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace basket.Basket.Features.UpdateItemPriceInBasket
{
    public record UpdateItemPriceInBasketCommand(Guid ProductId, decimal NewPrice) : IRequest<UpdateItemPriceInBasketResult>;
    public record UpdateItemPriceInBasketResult(bool IsSuccess);
    public class UpdateItemPriceInBasketCommandValidator : AbstractValidator<UpdateItemPriceInBasketCommand>
    {
        public UpdateItemPriceInBasketCommandValidator()
        {
            RuleFor(command => command.ProductId)
                .NotEmpty()
                .WithMessage("Product id is required to identify the product in the shopping cart.");
            RuleFor(command => command.NewPrice)
                .GreaterThanOrEqualTo(0)
                .WithMessage("New price must be greater than or equal to zero.");
        }
    }
    public class UpdateItemPriceInBasketHandler(BasketDbContext dbContext) : IRequestHandler<UpdateItemPriceInBasketCommand, UpdateItemPriceInBasketResult>
    {
        public async Task<UpdateItemPriceInBasketResult> Handle(UpdateItemPriceInBasketCommand command, CancellationToken cancellationToken)
        {
            var shoppingCarts = await dbContext.ShoppingCarts
                .Include(cart => cart.Items)
                .Where(cart => cart.Items.Any(item => item.ProductId == command.ProductId))
                .ToListAsync(cancellationToken);

            foreach(var shoppingCart in shoppingCarts)
            {
                shoppingCart.UpdateItemPrice(command.ProductId, command.NewPrice);
            }

            await dbContext.SaveChangesAsync(cancellationToken);

            return new UpdateItemPriceInBasketResult(IsSuccess: true);
        }
    }
}
