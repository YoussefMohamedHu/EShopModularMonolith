using basket.Data;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace basket.Basket.Features.DeleteBasket;

public record DeleteBasketCommand(string UserName) : IRequest<DeleteBasketResult>;
public record DeleteBasketResult(bool IsSuccess);

public class DeleteBasketCommandValidator : AbstractValidator<DeleteBasketCommand>
{
    public DeleteBasketCommandValidator()
    {
        RuleFor(c => c.UserName)
            .NotEmpty()
            .WithMessage("User name is required to delete a basket.");
    }
}

public class DeleteBasketHandler(BasketDbContext dbContext) : IRequestHandler<DeleteBasketCommand, DeleteBasketResult>
{
    async Task<DeleteBasketResult> IRequestHandler<DeleteBasketCommand, DeleteBasketResult>.Handle(DeleteBasketCommand command, CancellationToken cancellationToken)
    {
        var shoppingCart = await dbContext.ShoppingCarts
            .SingleOrDefaultAsync(cart => cart.UserName == command.UserName, cancellationToken);

        if (shoppingCart is null)
        {
            return new DeleteBasketResult(false);
        }

        dbContext.ShoppingCarts.Remove(shoppingCart);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new DeleteBasketResult(true);
    }
}
