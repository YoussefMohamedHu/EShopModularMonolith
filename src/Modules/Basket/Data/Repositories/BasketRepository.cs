using basket.Basket.Models;
using Microsoft.EntityFrameworkCore;
using Modules.Basket.Data.Repositories;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace basket.Data.Repositories;

public class BasketRepository(BasketDbContext _dbContext) : IBasketRepository
{
    public async Task<ShoppingCart> CreateBasket(ShoppingCart basket, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(basket);
        await _dbContext.ShoppingCarts.AddAsync(basket);
        return basket;
    }

    public async Task<bool> DeleteBasket(string userName, CancellationToken cancellationToken)
    {
        var basket = await GetBasket(userName, false, cancellationToken);

        if (basket is null)
        {
            return false;
        }
         _dbContext.ShoppingCarts.Remove(basket);
        return true;
    }

    public async Task<ShoppingCart?> GetBasket(string userName, bool asNoTracking = true, CancellationToken cancellationToken = default)
    {
        IQueryable<ShoppingCart> query = _dbContext.ShoppingCarts
            .Include(sc => sc.Items)
            .Where(sc => sc.UserName == userName);

        if (asNoTracking)
        {
            query = query.AsNoTracking();
        }

        var basket = await query.SingleOrDefaultAsync(cancellationToken);

        return basket;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken, string? userName = null)
        => await _dbContext.SaveChangesAsync(cancellationToken);
}
