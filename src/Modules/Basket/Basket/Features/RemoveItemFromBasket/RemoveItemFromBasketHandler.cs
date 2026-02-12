using basket.Data;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace basket.Basket.Features.RemoveItemFromBasket;

public record RemoveItemFromBasketCommand(string UserName, Guid ItemId) : IRequest<RemoveItemFromBasketResult>;
public record RemoveItemFromBasketResult(bool IsSuccess);

public class RemoveItemFromBasketCommandValidator : AbstractValidator<RemoveItemFromBasketCommand>
{
    public RemoveItemFromBasketCommandValidator()
    {
        RuleFor(command => command.UserName)
            .NotEmpty()
            .WithMessage("User name is required for the shopping cart.");

        RuleFor(command => command.ItemId)
            .NotEmpty()
            .WithMessage("Item id is required to identify the shopping cart item.");
    }
}

public class RemoveItemFromBasketHandler(BasketDbContext dbContext) : IRequestHandler<RemoveItemFromBasketCommand, RemoveItemFromBasketResult>
{
    public async Task<RemoveItemFromBasketResult> Handle(RemoveItemFromBasketCommand command, CancellationToken cancellationToken)
    {
        var shoppingCart = await dbContext.ShoppingCarts
            .Include(cart => cart.Items)
            .SingleOrDefaultAsync(cart => cart.UserName == command.UserName, cancellationToken);

        if (shoppingCart is null)
        {
            return new RemoveItemFromBasketResult(false);
        }

        var removed = shoppingCart.RemoveItemById(command.ItemId);

        if (!removed)
        {
            return new RemoveItemFromBasketResult(false);
        }
        await dbContext.SaveChangesAsync(cancellationToken);

        return new RemoveItemFromBasketResult(true);
    }
}
