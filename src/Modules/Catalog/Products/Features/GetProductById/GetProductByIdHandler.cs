using catalog.Data;
using catalog.Products.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace catalog.Products.Features.GetProductById
{
    record GetProductByIdQuery(Guid productId) : IRequest<GetProductByIdResult>;
    record GetProductByIdResult(ProductDto Product);
    public class GetProductByIdHandler(CatalogDbContext dbContext) : IRequestHandler<GetProductByIdQuery, GetProductByIdResult>
    {
        async Task<GetProductByIdResult> IRequestHandler<GetProductByIdQuery, GetProductByIdResult>.Handle(GetProductByIdQuery query, CancellationToken cancellationToken)
        {
            var product = await dbContext.Products
                .AsNoTracking()
                .SingleOrDefaultAsync(p => p.Id == query.productId, cancellationToken);

            if (product is null)
            {
                throw new Exception($"Product with id {query.productId} not found.");
            }

            var productDto = new ProductDto(
                product.Id,
                product.Name,
                product.Description,
                product.Price,
                product.ImageFile,
                product.Category.ToList()
                );

            return new GetProductByIdResult(productDto);
        }
    }
}
