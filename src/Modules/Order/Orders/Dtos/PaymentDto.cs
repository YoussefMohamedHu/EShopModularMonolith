namespace order.Orders.Dtos
{
    public record PaymentDto
    {
        public string CardName { get; set; } = default!;
        public string CardNumber { get; set; } = default!;
        public string ExpirationDate { get; set; } = default!;
        public string CVV { get; set; } = default!;
    }
}
