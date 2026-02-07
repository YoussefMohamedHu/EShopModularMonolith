using Carter;
using catalog.Products.Dtos;
using catalog.Products.Features.CreateProduct;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace catalog.Products.Features.GetProducts
{
    public record GetProductsResponse(List<ProductDto> products);
    public class GetProductsEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/product", async (ISender sender) =>
            {
                var result = await sender.Send(new GetProductsQuery());
                var response = new GetProductsResponse(result.Products.ToList());
                return Results.Ok(response);

            }).WithName("GetProduct")
              .Produces<GetProductsResponse>(StatusCodes.Status200OK)
              .ProducesProblem(StatusCodes.Status400BadRequest)
              .WithSummary("Get products")
              .WithDescription("Get All Products");
            
        }
    }
}
