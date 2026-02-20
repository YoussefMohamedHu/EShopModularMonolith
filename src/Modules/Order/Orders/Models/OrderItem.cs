using Shared.Base.DDD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace order.Orders.Models
{
    public class OrderItem : Entity<Guid>
    {
        public Guid OrderId { get; private set; }
        public Guid ProductId { get; private set; }
        public string ProductName { get; private set; } = default!;
        public decimal Price { get; private set; }
        public int Quantity { get; private set; }
        public OrderItem(Guid id,Guid orderId, Guid productId, string productName, decimal price, int quantity)
        {
            Id = id;
            OrderId = orderId;
            ProductId = productId;
            ProductName = productName;
            Price = price;
            Quantity = quantity;
        }
    }
}
