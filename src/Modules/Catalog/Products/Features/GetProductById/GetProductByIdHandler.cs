using catalog.Data;
using Catalog.Contracts.Products.Dtos;
using Catalog.Contracts.Products.Features.GetProductById;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace catalog.Products.Features.GetProductById
{
    public class GetProductByIdHandler(CatalogDbContext dbContext) : IRequestHandler<GetProductByIdQuery, GetProductByIdResult>
    {
        public async Task<GetProductByIdResult> Handle(GetProductByIdQuery query, CancellationToken cancellationToken)
        {
            var product = await dbContext.Products
                .AsNoTracking()
                .SingleOrDefaultAsync(p => p.Id == query.ProductId, cancellationToken);

            if (product is null)
            {
                throw new Exception($"Product with id {query.ProductId} not found.");
            }

            var productDto = new ProductMinimalDataDto(
                product.Id,
                product.Name,
                product.Price
            );

            return new GetProductByIdResult(productDto);
        }
    }
}
