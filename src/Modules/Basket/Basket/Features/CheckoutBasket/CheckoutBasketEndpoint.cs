using basket.Basket.Dtos;
using Carter;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace basket.Basket.Features.CheckoutBasket;

public record CheckoutBasketRequest(BasketCheckoutDto CheckoutBasket);
public record CheckoutBasketResponse(bool IsSuccess);

public class CheckoutBasketEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/basket/checkout", async (CheckoutBasketRequest request, ISender sender) =>
        {
            var command = new CheckoutBasketCommand(request.CheckoutBasket);
            var result = await sender.Send(command);
            return Results.Ok(new CheckoutBasketResponse(result.IsSuccess));
        })
        .WithName("CheckoutBasket")
        .Produces<CheckoutBasketResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .WithSummary("Checkout basket")
        .WithDescription("Processes the checkout of a shopping basket and creates an order.");
    }
}
