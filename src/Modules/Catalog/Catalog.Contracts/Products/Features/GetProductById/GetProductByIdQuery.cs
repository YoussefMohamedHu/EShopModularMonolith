using Catalog.Contracts.Products.Dtos;
using MediatR;

namespace Catalog.Contracts.Products.Features.GetProductById;

public record GetProductByIdQuery(Guid ProductId) : IRequest<GetProductByIdResult>;

public record GetProductByIdResult(ProductMinimalDataDto Product);
