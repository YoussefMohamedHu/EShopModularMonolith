using basket.Basket.Dtos;
using Carter;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace basket.Basket.Features.AddItemIntoBasket;

public record AddItemIntoBasketRequest(ShoppingCartItemRequestDto Item);

public record AddItemIntoBasketResponse(Guid Id);

public class AddItemIntoBasketEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/basket/{userName}/items", async (string userName, AddItemIntoBasketRequest request, ISender sender) =>
        {
            var result = await sender.Send(new AddItemIntoBasketCommand(userName, request.Item));
            var response = new AddItemIntoBasketResponse(result.Id);
            return Results.Ok(response);
        });
    }
}
