using catalog.Data;
using catalog.Products.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;
using shared.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace catalog.Products.Features.GetProducts
{
    public record GetProductsQuery(PaginationRequest request) : IRequest<GetProductsResult>;
    public record GetProductsResult(PaginatedResult<ProductDto> Products);
    public class GetProductsHandler(CatalogDbContext dbContext) : IRequestHandler<GetProductsQuery, GetProductsResult>
    {
         
        async Task<GetProductsResult> IRequestHandler<GetProductsQuery, GetProductsResult>.Handle(GetProductsQuery query, CancellationToken cancellationToken)
        {
            int pageIndex = query.request.PageIndex;
            int pageSize = query.request.PageSize;
            int totalCount = await dbContext.Products.CountAsync(cancellationToken);

            var products = await dbContext.Products
                .AsNoTracking()
                .OrderBy(p => p.Name)
                .Skip(pageIndex * pageSize)
                .Take(pageSize)
                .Select(p => new ProductDto(
                p.Id,
                p.Name,
                p.Description,
                p.Price,
                p.ImageFile,
                p.Category.ToList()
            )).ToListAsync();

            return new GetProductsResult(
                new PaginatedResult<ProductDto>(
                    pageIndex,
                    pageSize,
                    totalCount,
                    products
                ));
        }
    }
}
