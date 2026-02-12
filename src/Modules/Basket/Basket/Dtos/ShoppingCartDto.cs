using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace basket.Basket.Dtos
{
    public record ShoppingCartDto(
        Guid Id,
        string UserName,
        List<ShoppingCartItemDto> Items
    );
}
