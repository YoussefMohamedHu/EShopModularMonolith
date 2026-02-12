using basket.Basket.Dtos;
using Carter;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System.Collections.Generic;

namespace basket.Basket.Features.CreateBasket;

public record CreateBasketRequest(List<ShoppingCartItemDto> Items);
public record CreateBasketResponse(Guid Id);

public class CreateBasketEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/basket/{userName}", async (string userName, CreateBasketRequest request, ISender sender) =>
        {
            var result = await sender.Send(new CreateBasketCommand(userName, request.Items ?? new List<ShoppingCartItemDto>()));
            var response = new CreateBasketResponse(result.Id);
            return Results.Created($"/basket/{response.Id}", response);
        })
        .WithName("CreateBasket")
        .Produces<CreateBasketResponse>(StatusCodes.Status201Created)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .WithSummary("Creates a shopping cart")
        .WithDescription("Creates a shopping cart for the provided user and items.");
    }
}
