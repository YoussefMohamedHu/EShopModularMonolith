using catalog.Data;
using catalog.Products.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace catalog.Products.Features.GetProducts
{
    public record GetProductsQuery() : IRequest<GetProductsResult>;
    public record GetProductsResult(IEnumerable<ProductDto> Products);
    public class GetProductsHandler(CatalogDbContext dbContext) : IRequestHandler<GetProductsQuery, GetProductsResult>
    {
         
        async Task<GetProductsResult> IRequestHandler<GetProductsQuery, GetProductsResult>.Handle(GetProductsQuery query, CancellationToken cancellationToken)
        {
            var products = await dbContext.Products
                .AsNoTracking()
                .OrderBy(p => p.Name)
                .Select(p => new ProductDto(
                p.Id,
                p.Name,
                p.Description,
                p.Price,
                p.ImageFile,
                p.Category.ToList()
            )).ToListAsync();

            return new GetProductsResult(products);
        }
    }
}
