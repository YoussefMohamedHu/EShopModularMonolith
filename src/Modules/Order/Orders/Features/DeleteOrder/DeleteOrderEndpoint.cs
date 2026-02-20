using Carter;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace order.Orders.Features.DeleteOrder
{
    public record DeleteOrderResponse(bool IsSuccess);

    public class DeleteOrderEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapDelete("/order/{id:guid}", async (Guid id, ISender sender) =>
            {
                var result = await sender.Send(new DeleteOrderCommand(id));

                if (!result.IsSuccess)
                {
                    return Results.NotFound();
                }

                return Results.Ok(new DeleteOrderResponse(true));
            })
            .WithName("DeleteOrder")
            .Produces<DeleteOrderResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Deletes an order")
            .WithDescription("Deletes the specified order by id.");
        }
    }
}
