using catalog.Data;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace catalog.Products.Features.DeleteProduct
{
    public record DeleteProductCommand(Guid productId) : IRequest<DeleteProductResult>;
    public record DeleteProductResult(bool isSuccess);
    public class DeleteProductHandler(CatalogDbContext dbContext) : IRequestHandler<DeleteProductCommand,DeleteProductResult>
    {
        async Task<DeleteProductResult> IRequestHandler<DeleteProductCommand, DeleteProductResult>.Handle(DeleteProductCommand command, CancellationToken cancellationToken)
        {
            var product = await dbContext.Products.FindAsync([command.productId], cancellationToken);

            if(product is null)
            {
                throw new Exception($"Product with id {command.productId} not found.");
            }

            dbContext.Products.Remove(product);
            await dbContext.SaveChangesAsync(cancellationToken);

            return new DeleteProductResult(true);
        }

    }
}
