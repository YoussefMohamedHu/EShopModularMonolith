namespace order.Orders.Dtos
{
    public record AddressDto
    {
        public string? EmailAddress { get; set; }
        public string AddressLine { get; set; } = default!;
        public string City { get; set; } = default!;
        public string Country { get; set; } = default!;
    }
}
