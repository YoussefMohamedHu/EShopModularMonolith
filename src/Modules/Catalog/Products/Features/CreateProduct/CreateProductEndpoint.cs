using Carter;
using catalog.Products.Dtos;
using catalog.Products.Features.CreateProduct;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace catalog.Products.Features.CreateProduct
{
    public record CreateProductRequest(ProductDto product);
    public record CreateProductResponse(Guid Id);

    public class CreateProductEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("/product", async (CreateProductRequest request, ISender sender) =>
            {
                var result = await sender.Send(new CreateProductCommand(request.product));

                var response = new CreateProductResponse(result.Id);

                return Results.Created($"/product/{response.Id}", response);

            }
            ).WithName("CreateProduct")
            .Produces<CreateProductResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Creates product")
            .WithDescription("Creates a new product");
        }
    }
}
