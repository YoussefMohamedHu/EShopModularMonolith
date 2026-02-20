namespace order.Orders.Dtos
{
    public record OrderDto
    {
        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }
        public string OrderName { get; set; } = default!;
        public AddressDto ShippingAddress { get; set; } = default!;
        public PaymentDto Payment { get; set; } = default!;
        public List<OrderItemDto> Items { get; set; } = new();
        public decimal TotalPrice { get; set; }
    }
}
