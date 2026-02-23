using basket.Basket.Dtos;
using basket.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Modules.Basket.Data.Repositories;

namespace basket.Basket.Features.GetBasket;

public record GetBasketQuery(string UserName) : IRequest<GetBasketResult>;
public record GetBasketResult(ShoppingCartDto? ShoppingCart);

public class GetBasketHandler(IBasketRepository basketRepository) : IRequestHandler<GetBasketQuery, GetBasketResult>
{
    async Task<GetBasketResult> IRequestHandler<GetBasketQuery, GetBasketResult>.Handle(GetBasketQuery query, CancellationToken cancellationToken)
    {
        var shoppingCart = await basketRepository.GetBasket(query.UserName, true, cancellationToken);

        if (shoppingCart is null)
        {
            return new GetBasketResult(null);
        }

        var itemDtos = shoppingCart.Items.Select(item => new ShoppingCartItemDto(
            item.Id,
            item.ShoppingCartId,
            item.ProductId,
            item.ProductName,
            item.Quantity,
            item.Price,
            item.Color
        )).ToList();

        var resultDto = new ShoppingCartDto(
            shoppingCart.Id,
            shoppingCart.UserName,
            itemDtos
        );

        return new GetBasketResult(resultDto);
    }
}
