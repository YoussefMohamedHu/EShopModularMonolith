using basket.Basket.Models;
using basket.Data.JsonConverters;
using Microsoft.Extensions.Caching.Distributed;
using Modules.Basket.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace basket.Data.Repositories
{
    public class CachedBasketRepository(IBasketRepository innerRepository, IDistributedCache cache) : IBasketRepository
    {
        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true,
            Converters =
            {
                new ShoppingCartJsonConverter(),
                new ShoppingCartItemJsonConverter()
            }
        };

        public async Task<ShoppingCart> CreateBasket(ShoppingCart basket, CancellationToken cancellationToken)
        {
            var createdBasket = await innerRepository.CreateBasket(basket, cancellationToken);

            return createdBasket;
        }

        public async Task<bool> DeleteBasket(string userName, CancellationToken cancellationToken)
        {
            await cache.RemoveAsync(userName, cancellationToken);
            return await innerRepository.DeleteBasket(userName, cancellationToken);
        }

        public async Task<ShoppingCart?> GetBasket(string userName, bool asNoTracking = true, CancellationToken cancellationToken = default)
        {
            if (!asNoTracking)
            {
                return await innerRepository.GetBasket(userName, false, cancellationToken);
            }

            var cacheKey = $"basket:{userName}";

            var cachedBasket = await cache.GetStringAsync(cacheKey, cancellationToken);

            if (!string.IsNullOrEmpty(cachedBasket))
            {
                return JsonSerializer.Deserialize<ShoppingCart>(cachedBasket, JsonOptions);
            }
            else
            {
                var basket = await innerRepository.GetBasket(userName, true, cancellationToken);
                await cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(basket, JsonOptions));
                return basket;
            }
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken, string? userName = null)
        {
            if (userName is not null)
            {
                var cacheKey = $"basket:{userName}";

                await cache.RemoveAsync(cacheKey, cancellationToken);
            }

            return await innerRepository.SaveChangesAsync(cancellationToken, userName);
            
        }
    }
}
