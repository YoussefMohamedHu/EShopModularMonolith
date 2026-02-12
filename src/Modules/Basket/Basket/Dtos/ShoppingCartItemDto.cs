using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace basket.Basket.Dtos
{
    public record ShoppingCartItemDto(
        Guid Id,
        Guid ShoppingCartId,
        Guid ProductId,
        string ProductName,
        int Quantity,
        decimal Price,
        string Color
    );
}
