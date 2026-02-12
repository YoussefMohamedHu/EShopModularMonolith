using Carter;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System;

namespace basket.Basket.Features.RemoveItemFromBasket;

public record RemoveItemFromBasketResponse(bool IsSuccess);

public class RemoveItemFromBasketEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapDelete("/basket/{userName}/items/{itemId}", async (string userName, Guid itemId, ISender sender) =>
        {
            var result = await sender.Send(new RemoveItemFromBasketCommand(userName, itemId));

            if (!result.IsSuccess)
            {
                return Results.NotFound();
            }

            return Results.Ok(new RemoveItemFromBasketResponse(true));
        })
        .WithName("RemoveItemFromBasket")
        .Produces<RemoveItemFromBasketResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .WithSummary("Removes an item from the shopping cart")
        .WithDescription("Removes the specified item from the user's shopping cart.");
    }
}
