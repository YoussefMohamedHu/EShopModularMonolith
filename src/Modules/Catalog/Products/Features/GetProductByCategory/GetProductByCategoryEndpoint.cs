using Carter;
using catalog.Products.Dtos;
using catalog.Products.Features.GetProductById;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace catalog.Products.Features.GetProductByCategory
{
    //public record GetProductByCategoryRequest(string Category);
    public record GetProductByCategoryResponse(IEnumerable<ProductDto> Products);
    public class GetProductByCategoryEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/product/category/{category}", async (string category, ISender sender) =>
            {
                var result = await sender.Send(new GetProductByCategoryQuery(category));
                var response = new GetProductByCategoryResponse(result.products);

                return Results.Ok(response);
            }).WithName("GetProductByCategory")
              .Produces<GetProductByCategoryResponse>(StatusCodes.Status200OK)
              .ProducesProblem(StatusCodes.Status400BadRequest)
              .Produces(StatusCodes.Status404NotFound)
              .WithSummary("Get product by Category")
              .WithDescription("Get Product by his Category");
        }
    }
}
