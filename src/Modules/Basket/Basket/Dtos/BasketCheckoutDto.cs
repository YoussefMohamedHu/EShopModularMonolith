namespace basket.Basket.Dtos;

public class BasketCheckoutDto
{
    public string UserName { get; set; }
    public Guid CustomerId { get; set; }
    public decimal TotalPrice { get; set; }

    // Payment
    public string CardName { get; set; }
    public string CardNumber { get; set; }
    public string ExpirationDate { get; set; }
    public string CVV { get; set; }

    // Address
    public string EmailAddress { get; set; }
    public string AddressLine { get; set; }
    public string City { get; set; }
    public string Country { get; set; }
}

