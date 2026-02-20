namespace order.Orders.ValueObjects
{
    public record Address
    {
        public string? EmailAddress { get; private set; }
        public string AddressLine { get; private set; }
        public string City { get; private set; }
        public string Country { get; private set; }

        private Address() { }

        private Address(string? emailAddress, string addressLine, string city, string country)
        {
            EmailAddress = emailAddress;
            AddressLine = addressLine;
            City = city;
            Country = country;
        }

        public static Address Of(string? emailAddress, string addressLine, string city, string country)
        {
            return new Address(emailAddress, addressLine, city, country);
        }
    }
}
