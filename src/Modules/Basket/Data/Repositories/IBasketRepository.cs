using basket.Basket.Models;

namespace Modules.Basket.Data.Repositories;

public interface IBasketRepository
{
    Task<ShoppingCart> CreateBasket(ShoppingCart basket, CancellationToken cancellationToken);
    Task<bool> DeleteBasket(string userName, CancellationToken cancellationToken);
    Task<ShoppingCart?> GetBasket(string userName, bool asNoTracking = true, CancellationToken cancellationToken = default);
    Task<int> SaveChangesAsync(CancellationToken cancellationToken, string? userName = null);
}
