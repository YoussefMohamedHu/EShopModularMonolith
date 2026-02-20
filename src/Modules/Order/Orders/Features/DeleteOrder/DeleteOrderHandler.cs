using FluentValidation;
using MediatR;
using order.Data;

namespace order.Orders.Features.DeleteOrder
{
    public record DeleteOrderCommand(Guid OrderId) : IRequest<DeleteOrderResult>;
    public record DeleteOrderResult(bool IsSuccess);

    public class DeleteOrderCommandValidator : AbstractValidator<DeleteOrderCommand>
    {
        public DeleteOrderCommandValidator()
        {
            RuleFor(c => c.OrderId)
                .NotEmpty().WithMessage("Order id is required.");
        }
    }

    public class DeleteOrderHandler(OrderDbContext dbContext) : IRequestHandler<DeleteOrderCommand, DeleteOrderResult>
    {
        public async Task<DeleteOrderResult> Handle(DeleteOrderCommand command, CancellationToken cancellationToken)
        {
            var order = await dbContext.Orders.FindAsync([command.OrderId], cancellationToken);

            if (order is null)
            {
                return new DeleteOrderResult(false);
            }

            dbContext.Orders.Remove(order);
            await dbContext.SaveChangesAsync(cancellationToken);

            return new DeleteOrderResult(true);
        }
    }
}
