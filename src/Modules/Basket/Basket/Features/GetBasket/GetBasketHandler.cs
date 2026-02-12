using basket.Basket.Dtos;
using basket.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace basket.Basket.Features.GetBasket;

public record GetBasketQuery(string UserName) : IRequest<GetBasketResult>;
public record GetBasketResult(ShoppingCartDto? ShoppingCart);

public class GetBasketHandler(BasketDbContext dbContext) : IRequestHandler<GetBasketQuery, GetBasketResult>
{
    async Task<GetBasketResult> IRequestHandler<GetBasketQuery, GetBasketResult>.Handle(GetBasketQuery query, CancellationToken cancellationToken)
    {
        var shoppingCart = await dbContext.ShoppingCarts
            .Include(cart => cart.Items)
            .AsNoTracking()
            .SingleOrDefaultAsync(cart => cart.UserName == query.UserName, cancellationToken);

        if (shoppingCart is null)
        {
            throw new Exception($"Shopping cart for user '{query.UserName}' not found.");
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
