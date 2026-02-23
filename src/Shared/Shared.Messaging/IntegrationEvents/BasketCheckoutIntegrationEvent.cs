using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Messaging.IntegrationEvents
{
    public record BasketItemEventDto
    {
        public Guid ProductId { get; set; }
        public string ProductName { get; set; } = default!;
        public decimal Price { get; set; }
        public int Quantity { get; set; }
    }

    public record BasketCheckoutIntegrationEvent : IntegrationEvent
    {
        public string UserName { get; set; } = default!;
        public Guid CustomerId { get; set; } = default!;
        public decimal TotalPrice { get; set; } = default!;

        // Basket items
        public List<BasketItemEventDto> Items { get; set; } = new();

        // Payment properties
        public string CardName { get; set; } = default!;
        public string CardNumber { get; set; } = default!;
        public string ExpirationDate { get; set; } = default!;
        public string CVV { get; set; } = default!;

        // Address properties
        public string? EmailAddress { get; set; }
        public string AddressLine { get; set; } = default!;
        public string City { get; set; } = default!;
        public string Country { get; set; } = default!;
    }
}
