using basket.Basket.Dtos;
using Carter;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace basket.Basket.Features.GetBasket;

public record GetBasketResponse(ShoppingCartDto ShoppingCart);

public class GetBasketEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/basket/{userName}", async (string userName, ISender sender) =>
        {
            var result = await sender.Send(new GetBasketQuery(userName));
            if (result.ShoppingCart is null)
            {
                return Results.NotFound();
            }

            return Results.Ok(new GetBasketResponse(result.ShoppingCart));
        })
        .WithName("GetBasket")
        .Produces<GetBasketResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .WithSummary("Gets a shopping cart")
        .WithDescription("Returns the shopping cart for the specified user name.");
    }
}
