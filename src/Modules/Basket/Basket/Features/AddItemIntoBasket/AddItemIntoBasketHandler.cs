using basket.Basket.Dtos;
using basket.Data;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Modules.Basket.Data.Repositories;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace basket.Basket.Features.AddItemIntoBasket;

public record AddItemIntoBasketCommand(string UserName, ShoppingCartItemDto Item) : IRequest<AddItemIntoBasketResult>;
public record AddItemIntoBasketResult(Guid Id);

public class AddItemIntoBasketCommandValidator : AbstractValidator<AddItemIntoBasketCommand>
{
    public AddItemIntoBasketCommandValidator()
    {
        RuleFor(sc => sc.UserName).NotEmpty().WithMessage("User name is required for the shopping cart.");
        RuleFor(sc => sc.Item.ProductId).NotEmpty().WithMessage("Product id is required.");
        RuleFor(sc => sc.Item.Quantity).GreaterThan(0).WithMessage("Quantity must be greater than zero.");
    }
}

public class AddItemIntoBasketHandler(IBasketRepository basketRepository) : IRequestHandler<AddItemIntoBasketCommand, AddItemIntoBasketResult>
{
    public async Task<AddItemIntoBasketResult> Handle(AddItemIntoBasketCommand command, CancellationToken cancellationToken)
    {
        var shoppingCart = await basketRepository.GetBasket(command.UserName,false, cancellationToken);

        if (shoppingCart == null)
        {
            throw new Exception($"Shopping cart for user '{command.UserName}' not found.");
        }

        shoppingCart.AddItem(
            command.Item.ProductId,
            command.Item.Quantity,
            command.Item.Color,
            command.Item.Price,
            command.Item.ProductName);

        await basketRepository.SaveChangesAsync(cancellationToken, command.UserName);

        return new AddItemIntoBasketResult(shoppingCart.Id);
    }
}
