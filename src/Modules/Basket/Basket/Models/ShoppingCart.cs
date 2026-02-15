using Shared.Base.DDD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace basket.Basket.Models
{
    public class ShoppingCart : Aggregate<Guid>
    {
        public string UserName { get; private set; } = default!;
        private readonly List<ShoppingCartItem> _items = new List<ShoppingCartItem>();
        public IReadOnlyList<ShoppingCartItem> Items => _items.AsReadOnly();
        public decimal TotalPrice => _items.Sum(item => item.Price * item.Quantity);

        public static ShoppingCart CreateShoppingCart(Guid id, string userName)
        {
            ArgumentException.ThrowIfNullOrEmpty(userName, nameof(userName));

            return new ShoppingCart
            {
                Id = id,
                UserName = userName
            };
        }

        public void AddItem(Guid productId, int quantity, string color, decimal price, string productName)
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(quantity, nameof(quantity));
            ArgumentOutOfRangeException.ThrowIfNegative(price, nameof(price));

            var existingItem = _items.FirstOrDefault(item => item.ProductId == productId && item.Color == color);
            if (existingItem != null)
            {
                existingItem.Quantity += quantity;
            }
            else
            {
                var newItem = new ShoppingCartItem(Id, productId, quantity, color, price, productName);
                _items.Add(newItem);
            }
        }

        public void RemoveItem(Guid productId, string color)
        {
            var existingItem = _items.FirstOrDefault(item => item.ProductId == productId && item.Color == color);
            if (existingItem != null)
            {
                _items.Remove(existingItem);
            }
        }

        public bool RemoveItemById(Guid itemId)
        {
            var existingItem = _items.FirstOrDefault(item => item.Id == itemId);
            if (existingItem == null)
            {
                return false;
            }

            _items.Remove(existingItem);
            return true;
        }

        public void UpdateItemPrice(Guid productId , decimal newPrice)
        {
            ArgumentOutOfRangeException.ThrowIfNegative(newPrice, nameof(newPrice));
            var existingItem = _items.FirstOrDefault(item => item.ProductId == productId);
            if (existingItem != null)
            {
                existingItem.UpdatePrice(newPrice);
            }
        }
    }
}
