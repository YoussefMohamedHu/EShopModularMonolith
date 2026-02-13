using basket.Data;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Modules.Basket.Data.Repositories;
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

public class DeleteBasketHandler(IBasketRepository basketRepository) : IRequestHandler<DeleteBasketCommand, DeleteBasketResult>
{
    async Task<DeleteBasketResult> IRequestHandler<DeleteBasketCommand, DeleteBasketResult>.Handle(DeleteBasketCommand command, CancellationToken cancellationToken)
    {
        
        await basketRepository.DeleteBasket(command.UserName,cancellationToken);
        await basketRepository.SaveChangesAsync(cancellationToken,command.UserName);

        return new DeleteBasketResult(true);
    }
}
