using basket.Basket.Dtos;
using basket.Basket.Models;
using basket.Data;
using Catalog.Contracts.Products.Features.GetProductById;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Modules.Basket.Data.Repositories;
using System.Collections.Generic;

namespace basket.Basket.Features.CreateBasket;

public record CreateBasketCommand(string UserName, List<ShoppingCartItemRequestDto> Items) : IRequest<CreateBasketResult>;
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
            items.RuleFor(item => item.Color)
                .NotEmpty().WithMessage("Item color is required.");
        });
    }
}

public class CreateBasketHandler(
    IBasketRepository basketRepository,
    ISender sender) : IRequestHandler<CreateBasketCommand, CreateBasketResult>
{
    public async Task<CreateBasketResult> Handle(CreateBasketCommand command, CancellationToken cancellationToken)
    {
        var userName = command.UserName;
        var existingCart = await basketRepository.GetBasket(userName, asNoTracking: true, cancellationToken);

        if (existingCart is not null)
        {
            throw new InvalidOperationException($"A shopping cart already exists for user '{userName}'.");
        }

        var shoppingCart = ShoppingCart.CreateShoppingCart(Guid.NewGuid(), userName);

        // Fetch product data from Catalog for each item
        foreach (var item in command.Items)
        {
            var productResult = await sender.Send(
                new GetProductByIdQuery(item.ProductId),
                cancellationToken);

            if (productResult.Product is null)
            {
                throw new Exception($"Product with id '{item.ProductId}' not found in catalog.");
            }

            shoppingCart.AddItem(
                item.ProductId,
                item.Quantity,
                item.Color,
                productResult.Product.Price,      // FROM CATALOG (source of truth)
                productResult.Product.Name);      // FROM CATALOG (source of truth)
        }

        await basketRepository.CreateBasket(shoppingCart, cancellationToken);
        await basketRepository.SaveChangesAsync(cancellationToken);

        return new CreateBasketResult(shoppingCart.Id);
    }
}
