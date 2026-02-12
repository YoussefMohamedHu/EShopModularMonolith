using basket.Basket.Dtos;
using basket.Basket.Models;
using basket.Data;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace basket.Basket.Features.CreateBasket;

public record CreateBasketCommand(string UserName, List<ShoppingCartItemDto> Items) : IRequest<CreateBasketResult>;
public record CreateBasketResult(Guid Id);

public class CreateBasketCommandValidator : AbstractValidator<CreateBasketCommand>
{
    public CreateBasketCommandValidator()
    {
        RuleFor(c => c.UserName)
            .NotEmpty().WithMessage("User name is required for the shopping cart.");

        RuleFor(c => c.Items)
            .NotNull().WithMessage("Shopping cart items are required.")
            .Must(items => items?.Count > 0).WithMessage("Shopping cart must contain at least one item.");

        RuleForEach(c => c.Items).ChildRules(items =>
        {
            items.RuleFor(item => item.ProductId)
                .NotEmpty().WithMessage("Product id is required.");
            items.RuleFor(item => item.Quantity)
                .GreaterThan(0).WithMessage("Quantity must be greater than zero.");
            items.RuleFor(item => item.Price)
                .GreaterThanOrEqualTo(0).WithMessage("Price must be a non-negative value.");
            items.RuleFor(item => item.Color)
                .NotEmpty().WithMessage("Item color is required.");
            items.RuleFor(item => item.ProductName)
                .NotEmpty().WithMessage("Product name is required.");
        });
    }
}

public class CreateBasketHandler(BasketDbContext dbContext) : IRequestHandler<CreateBasketCommand, CreateBasketResult>
{
    async Task<CreateBasketResult> IRequestHandler<CreateBasketCommand, CreateBasketResult>.Handle(CreateBasketCommand command, CancellationToken cancellationToken)
    {
        var userName = command.UserName;
        var existingCart = await dbContext.ShoppingCarts
            .AsNoTracking()
            .FirstOrDefaultAsync(cart => cart.UserName == userName, cancellationToken);

        if (existingCart is not null)
        {
            throw new InvalidOperationException($"A shopping cart already exists for user '{userName}'.");
        }

        var shoppingCart = ShoppingCart.CreateShoppingCart(Guid.NewGuid(), userName);

        foreach (var item in command.Items)
        {
            shoppingCart.AddItem(item.ProductId, item.Quantity, item.Color, item.Price, item.ProductName);
        }

        dbContext.ShoppingCarts.Add(shoppingCart);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new CreateBasketResult(shoppingCart.Id);
    }
}
