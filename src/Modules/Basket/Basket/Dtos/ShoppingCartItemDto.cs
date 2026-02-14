namespace basket.Basket.Dtos;

// Used for API Requests - client only sends minimal data
public record ShoppingCartItemRequestDto(
    Guid ProductId,
    int Quantity,
    string Color
);

// Used for API Responses - server returns full data from database
public record ShoppingCartItemDto(
    Guid Id,
    Guid ShoppingCartId,
    Guid ProductId,
    string ProductName,
    int Quantity,
    decimal Price,
    string Color
);
