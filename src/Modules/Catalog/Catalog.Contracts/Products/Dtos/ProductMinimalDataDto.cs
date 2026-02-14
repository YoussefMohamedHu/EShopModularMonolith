namespace Catalog.Contracts.Products.Dtos;

public record ProductMinimalDataDto(
    Guid Id,
    string Name,
    decimal Price
);
