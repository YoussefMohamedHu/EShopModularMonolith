using catalog.Data;
using catalog.Products.Dtos;
using catalog.Products.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace catalog.Products.Features.CreateProduct
{
    public record CreateProductCommand(ProductDto product) : IRequest<CreateProductResult>;

    public record CreateProductResult(Guid Id);
    public class CreateProductHandler(CatalogDbContext dbContext) : IRequestHandler<CreateProductCommand, CreateProductResult>
    {

         async Task<CreateProductResult> IRequestHandler<CreateProductCommand, CreateProductResult>.Handle(CreateProductCommand command, CancellationToken cancellationToken)
        {
            var product = CreateNewProduct(command.product);

            dbContext.Products.Add(product);
            await dbContext.SaveChangesAsync(cancellationToken);

            return new CreateProductResult(product.Id);
        }
        private Product CreateNewProduct(ProductDto productDto)
        {
            return Product.Create(
                Guid.NewGuid(),
                productDto.Name,
                productDto.Category,
                productDto.Description,
                productDto.ImageFile,
                productDto.Price);
        }

    }
}
