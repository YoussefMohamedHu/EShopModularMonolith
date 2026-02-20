using order.Orders.Events;
using order.Orders.ValueObjects;
using Shared.Base.DDD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace order.Orders.Models
{
    public class Order : Aggregate<Guid>
    {
        private readonly List<OrderItem> _items = new List<OrderItem>();
        public IReadOnlyList<OrderItem> Items => _items.AsReadOnly();   

        public Guid CustomerId { get; private set; } = default!;
        public string OrderName { get; private set; } = default!;
        public Address ShippingAddress { get; private set; } = default!;
        public Payment Payment { get; private set; } = default!;
        public decimal TotalPrice => _items.Sum(i => i.Price * i.Quantity);

        public static Order Create(Guid id, Guid customerId, string orderName, Address shippingAddress, Address billingAddress, Payment payment)
        {
            var order = new Order
            {
                Id = id,
                CustomerId = customerId,
                OrderName = orderName,
                ShippingAddress = shippingAddress,
                Payment = payment
            };

            order.AddDomainEvent(new OrderCreatedEvent(order));
            return order;
        }

        public void AddItem(Guid id, Guid productId, string productName, decimal price, int quantity)
        {
            ArgumentException.ThrowIfNullOrEmpty(productName, nameof(productName));
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(price, nameof(price));
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(quantity, nameof(quantity));

            var existingItem = _items.FirstOrDefault(i => i.ProductId == productId);
            if (existingItem != null)
            {
                // If the item already exists, update the quantity and price
                var updatedItem = new OrderItem(Guid.NewGuid(),orderId: id, productId, productName, price, existingItem.Quantity + quantity);
                _items.Remove(existingItem);
                _items.Add(updatedItem);
            }
            else
            {
                // Otherwise, add a new item to the order
                var newItem = new OrderItem(Guid.NewGuid(),orderId: id, productId, productName, price, quantity);
                _items.Add(newItem);
            }
        }

        public void RemoveItem(Guid id)
        {
            var item = _items.Where(_items => _items.Id == id).SingleOrDefault();
            if (item is null )
            {
                throw new InvalidOperationException($"No order item found with id '{id}'.");
            }
            _items.Remove(item);
        }
    }
}
