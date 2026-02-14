using Carter;
using Catalog.Contracts.Products.Dtos;
using Catalog.Contracts.Products.Features.GetProductById;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace catalog.Products.Features.GetProductById;

public record GetProductByIdResponse(ProductMinimalDataDto Product);

public class GetProductByIdEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/product/{id:guid}", async (Guid id, ISender sender) =>
        {
            var result = await sender.Send(new GetProductByIdQuery(id));
            var response = new GetProductByIdResponse(result.Product);

            return Results.Ok(response);
        })
        .WithName("GetProductById")
        .Produces<GetProductByIdResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status404NotFound)
        .WithSummary("Get product by Id")
        .WithDescription("Get Product by his Id");
    }
}
