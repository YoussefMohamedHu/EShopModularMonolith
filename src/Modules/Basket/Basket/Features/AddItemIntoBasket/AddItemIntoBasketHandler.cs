using basket.Basket.Dtos;
using Catalog.Contracts.Products.Features.GetProductById;
using FluentValidation;
using MediatR;
using Modules.Basket.Data.Repositories;

namespace basket.Basket.Features.AddItemIntoBasket;

public record AddItemIntoBasketCommand(string UserName, ShoppingCartItemRequestDto Item) : IRequest<AddItemIntoBasketResult>;
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

public class AddItemIntoBasketHandler(
    IBasketRepository basketRepository,
    ISender sender) : IRequestHandler<AddItemIntoBasketCommand, AddItemIntoBasketResult>
{
    public async Task<AddItemIntoBasketResult> Handle(AddItemIntoBasketCommand command, CancellationToken cancellationToken)
    {
        var shoppingCart = await basketRepository.GetBasket(command.UserName, false, cancellationToken);

        if (shoppingCart == null)
        {
            throw new Exception($"Shopping cart for user '{command.UserName}' not found.");
        }

        // Fetch product data from Catalog module (source of truth)
        var productResult = await sender.Send(
            new GetProductByIdQuery(command.Item.ProductId),
            cancellationToken);

        if (productResult.Product is null)
        {
            throw new Exception($"Product with id '{command.Item.ProductId}' not found in catalog.");
        }

        // Use product data from Catalog, NOT from client
        shoppingCart.AddItem(
            command.Item.ProductId,
            command.Item.Quantity,
            command.Item.Color,
            productResult.Product.Price,      // FROM CATALOG (source of truth)
            productResult.Product.Name);      // FROM CATALOG (source of truth)

        await basketRepository.SaveChangesAsync(cancellationToken, command.UserName);

        return new AddItemIntoBasketResult(shoppingCart.Id);
    }
}
