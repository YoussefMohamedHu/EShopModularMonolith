using Carter;
using catalog.Products.Dtos;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace catalog.Products.Features.UpdateProduct
{
    public record UpdateProductRequest(ProductDto product);
    public record UpdateProductResponse(bool isSuccess);
    public class UpdateProductEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPut("/product/{id:guid}", async (UpdateProductRequest request, ISender sender) =>
            {
                var command = new UpdateProductCommand(request.product);
                var result = await sender.Send(command);
                return Results.Ok(new UpdateProductResponse(result.isSuccess));

            })
                .WithName("UpdateProduct")
                .Produces<UpdateProductResponse>(StatusCodes.Status200OK)
                .ProducesProblem(StatusCodes.Status400BadRequest)
                .ProducesProblem(StatusCodes.Status404NotFound)
                .WithSummary("Updates an existing product")
                .WithDescription("Updates an existing product with the provided information. The product ID must be specified in the URL and must match the ID in the request body.");
        }
    }
}
