using catalog.Data;
using catalog.Products.Dtos;
using catalog.Products.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace catalog.Products.Features.UpdateProduct
{
    public record UpdateProductCommand(ProductDto product) : IRequest<UpdateProductResult>;

    public record UpdateProductResult(bool isSuccess);
    public class UpdateProductHandler(CatalogDbContext dbContext) : IRequestHandler<UpdateProductCommand, UpdateProductResult>
    {

         async Task<UpdateProductResult> IRequestHandler<UpdateProductCommand, UpdateProductResult>.Handle(UpdateProductCommand command, CancellationToken cancellationToken)
        {
            var product = await dbContext.Products.FindAsync([ command.product.Id ], cancellationToken);

            if(product is null)
            {
                throw new Exception($"Product with id {command.product.Id} not found.");
            }
            UpdateProductWithNewValues(product, command.product);

            dbContext.Products.Update(product);
            await dbContext.SaveChangesAsync(cancellationToken);

            return new UpdateProductResult(true);
        }
        private void UpdateProductWithNewValues(Product newProduct,ProductDto oldProduct)
        {
            newProduct.Update(
                oldProduct.Name,
                oldProduct.Category,
                oldProduct.Description,
                oldProduct.ImageFile,
                oldProduct.Price
                );
        }

    }
}
