using catalog.Data;
using catalog.Products.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace catalog.Products.Features.GetProductByCategory
{
    record GetProductByCategoryQuery(string category) : IRequest<GetProductByCategoryResult>;
    record GetProductByCategoryResult(IEnumerable<ProductDto> products);
    public class GetProductByCategoryHandler(CatalogDbContext dbContext) : IRequestHandler<GetProductByCategoryQuery, GetProductByCategoryResult>
    {
        async Task<GetProductByCategoryResult> IRequestHandler<GetProductByCategoryQuery, GetProductByCategoryResult>.Handle(GetProductByCategoryQuery request, CancellationToken cancellationToken)
        {
            var products = await dbContext.Products
                .AsNoTracking()
                .OrderBy(p => p.Name)
                .Where(p => p.Category.Contains(request.category))
                .Select(p => new ProductDto(
                    p.Id,
                    p.Name,
                    p.Description,
                    p.Price,
                    p.ImageFile,
                    p.Category.ToList()
                )).ToListAsync(cancellationToken);

            return new GetProductByCategoryResult(products);
        }
    }

}
