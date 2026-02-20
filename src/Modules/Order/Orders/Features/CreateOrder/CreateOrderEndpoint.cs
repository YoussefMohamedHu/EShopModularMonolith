using Carter;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using order.Orders.Dtos;

namespace order.Orders.Features.CreateOrder
{
    public record CreateOrderRequest(OrderDto Order);
    public record CreateOrderResponse(Guid Id);

    public class CreateOrderEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("/order", async (CreateOrderRequest request, ISender sender) =>
            {
                var result = await sender.Send(new CreateOrderCommand(request.Order));
                var response = new CreateOrderResponse(result.Id);
                return Results.Created($"/order/{response.Id}", response);
            })
            .WithName("CreateOrder")
            .Produces<CreateOrderResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Creates an order")
            .WithDescription("Creates a new order with items, shipping, and payment details.");
        }
    }
}
